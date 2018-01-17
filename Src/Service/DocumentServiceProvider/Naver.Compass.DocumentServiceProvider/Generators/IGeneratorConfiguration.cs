using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public enum GeneratorType
    {
        Html,
        Word,
        Csv
    }

    public interface IGeneratorConfiguration : INamedObject
    {
        IGeneratorConfigurationSet GeneratorConfigurationSet { get; }

        GeneratorType GeneratorType { get; }
    }
}
