using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public interface IGeneratorConfigurations
    {
        IGeneratorConfigurationSet GeneratorConfigurationSet { get; }

        IGeneratorConfiguration GetGeneratorConfiguration(string configName);

        bool Contains(string configName);

        int Count { get; }

        IGeneratorConfiguration this[string configName] { get; }
    }
}
