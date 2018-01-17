using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace Naver.Compass.Common.CommonBase
{
    public class ProcessSharedMemory<T> where T : struct
    {
        // Constructor
        public ProcessSharedMemory(string name, int size)
        {
            smName = name;
            smSize = size;
        }

        // Methods
        public bool Open()
        {
            try
            {
                // Create named MMF
                mmf = MemoryMappedFile.CreateOrOpen(smName, smSize);

                // Create accessors to MMF
                accessor = mmf.CreateViewAccessor(0, smSize,
                               MemoryMappedFileAccess.ReadWrite);

                // Create lock
                smLock = new Mutex(true, "SM_LOCK", out locked);
            }
            catch
            {
                return false;
            }

            return true;
        }
        public void Close()
        {
            accessor.Dispose();
            mmf.Dispose();
            smLock.Close();
        }
        public T Data
        {
            get
            {
                T dataStruct;
                accessor.Read<T>(0, out dataStruct);
                return dataStruct;
            }
            set
            {
                smLock.WaitOne();
                accessor.Write<T>(0, ref value);
                smLock.ReleaseMutex();
            }
        }

        // Data
        private string smName;
        private Mutex smLock;
        private int smSize;
        private bool locked;
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor accessor;
    }
    public class ProcessSharedMemoryObj
    {
        // Constructor
        public ProcessSharedMemoryObj(string name, int size)
        {
            smName = name;
            smSize = size;
        }

        // Methods
        public bool Open()
        {
            try
            {
                // Create named MMF
                mmf = MemoryMappedFile.CreateOrOpen(smName, smSize);

                // Create lock
                smLock = new Mutex(true, "SM_LOCK", out locked);
            }
            catch
            {
                return false;
            }

            return true;
        }
        public void Close()
        {
            mmf.Dispose();
            smLock.Close();
        }
        public object Data
        {
            get
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    byte[] buffer = new byte[accessor.Capacity];
                    accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);
                    return ByteArrayToObject(buffer);
                }                      
            }
            set
            {
                //smLock.WaitOne();
                byte[] buffer = ObjectToByteArray(value);
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, buffer.Length))
                {
                    // Write the data
                    accessor.WriteArray<byte>(0, buffer, 0, buffer.Length);
                }
                //smLock.ReleaseMutex();
            }
        }

        // Data
        private string smName;
        private Mutex smLock;
        private int smSize;
        private bool locked;
        private MemoryMappedFile mmf;

        private object ByteArrayToObject(byte[] buffer)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();    // Create new BinaryFormatter
            MemoryStream memoryStream = new MemoryStream(buffer);       // Convert byte array to memory stream, set position to start
            return binaryFormatter.Deserialize(memoryStream);           // Deserializes memory stream into an object and return
        }

        private byte[] ObjectToByteArray(object inputObject)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();    // Create new BinaryFormatter
            MemoryStream memoryStream = new MemoryStream();             // Create target memory stream
            binaryFormatter.Serialize(memoryStream, inputObject);       // Convert object to memory stream
            return memoryStream.ToArray();                              // Return memory stream as byte array
        }
    }
}
