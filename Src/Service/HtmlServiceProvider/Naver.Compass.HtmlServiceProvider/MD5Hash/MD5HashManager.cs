using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Naver.Compass.Service.Html
{

    public static class MD5HashManager
    {
        #region public interface functions
        public static string GetHash(IWidgetStyle style, bool bIsPlaced,bool bIsSetTarget=false)
        {
            if (style == null)
            {
                return string.Empty;

            }
            SerialWidgetStyle target = new SerialWidgetStyle(style, bIsPlaced);
            string md5 = CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                style.MD5 = md5;
            }
            return md5;
        }
        public static string GetHash(IMasterStyle style, bool bIsPlaced,bool bIsSetTarget = false)
        {
            if (style == null)
            {
                return string.Empty;

            }
            SerialMasterStyle target = new SerialMasterStyle(style, bIsPlaced);
            string md5= CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                style.MD5 = md5;
            }
            return md5;
        }

        public static string GetHash(IWidget wdg, bool bIsSetTarget = false)
        {
            SerialWidgetBase target = CreateSerialWidget(wdg);
            if(target==null)
            {
                return string.Empty;

            }
            string md5= CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                wdg.MD5 = md5;
            }
            return md5;
        }
        public static string GetHash(IMaster wdg, bool bIsSetTarget = false)
        {
            SerialMaster target = CreateSerialMaster(wdg);
            if(target==null)
            {
                return string.Empty;

            }
            string md5= CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                wdg.MD5 = md5;
            }
            return md5;
        }

        public static string GetHash(IEmbeddedPage page, bool bIsSetTarget = false)
        {
            if(page.IsOpened==false)
            {
                page.Open();
            }

            SerialPage target = new SerialPage(page);
            string md5 = CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                page.MD5 = md5;
            }
            return md5;
        }
        public static string GetHash(IPage page, bool bIsSetTarget = false)
        {
            if (page.IsOpened == false)
            {
                page.Open();
            }

            SerialPage target = new SerialPage(page);
            string md5 = CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                page.MD5 = md5;
            }
            return md5;
        }
        public static string GetHash(IMasterPage page, bool bIsSetTarget = false)
        {
            if (page.IsOpened == false)
            {
                page.Open();
            }
            else
            {
                //MD5 code had been created.
                //if(string.IsNullOrEmpty(page.MD5)==false)
                //{
                //    return page.MD5;
                //}
            }

            //Create new MD% code now
            SerialPage target = new SerialPage(page);
            string md5 = CreaetMD5Hash(target, bIsSetTarget);
            if (bIsSetTarget == true)
            {
                page.MD5 = md5;
            }
            return md5;
        } 
        #endregion



        #region private functions
        private  static string CreaetMD5Hash(object target,bool bIsSetTarget)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);

                MD5CryptoServiceProvider MD5Srv = new MD5CryptoServiceProvider();
                byte[] hash = MD5Srv.ComputeHash(stream.GetBuffer());
                return BitConverter.ToString(hash);
            }
        }
        private static SerialWidgetBase CreateSerialWidget(IWidget wdg)
        {
            if (wdg == null)
            {
                return null;
            }

            switch (wdg.WidgetType)
            {
                case WidgetType.Shape:
                    return new SerialShape(wdg as IShape);
                case WidgetType.Button:
                    return new SerialButton(wdg as IButton);
                case WidgetType.Image:
                    return new SerialImage(wdg as IImage);
                case WidgetType.Line:
                    return new SerialLine(wdg as ILine);
                case WidgetType.HotSpot:
                    return new SerialHotSpot(wdg as IHotSpot);
                case WidgetType.TextField:
                    return new SerialTextField(wdg as ITextField);
                case WidgetType.TextArea:
                    return new SerialTextArea(wdg as ITextArea);
                case WidgetType.RadioButton:
                    return new SerialRadioButton(wdg as IRadioButton);
                case WidgetType.Checkbox:
                   return new SerialCheckbox(wdg as ICheckbox);
                case WidgetType.SVG:
                   return new SerialSVG(wdg as ISvg);
                case WidgetType.DropList:
                    return new SerialDropList(wdg as IDroplist);
                case WidgetType.ListBox:    
                   return new SerialListBox(wdg as IListBox);
                case WidgetType.Toast:
                    return new SerialToast(wdg as IToast);
                case WidgetType.HamburgerMenu:
                    return new SerialHamburgerMenu(wdg as IHamburgerMenu);
                case WidgetType.DynamicPanel:
                   return new SerialDynamicPanel(wdg as IDynamicPanel);


                case WidgetType.FlowShape:
                    return null;
                default:
                    return null;
            }
        }
        private static SerialMaster CreateSerialMaster(IMaster wdg)
        {
            if (wdg == null)
            {
                return null;
            }
            return new SerialMaster(wdg);  

        }
        #endregion
    }
}
