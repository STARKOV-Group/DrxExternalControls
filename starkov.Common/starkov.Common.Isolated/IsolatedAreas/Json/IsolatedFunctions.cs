using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using starkov.Common.Structures.Module;

namespace starkov.Common.Isolated.Json
{
  public class IsolatedFunctions
  {

    /// <summary>
    /// Сериализовать структуру с информацией о странице.
    /// </summary>
    /// <param name="data">Структура с информацией о странице.</param>
    /// <returns>Строка в виде Json.</returns>
    [Public]
    public string SerializePageStruct(IPageInfo data)
    {
      return JsonConvert.SerializeObject(data);
    }
    
    /// <summary>
    /// Десериализовать строку Json с информацией о странице.
    /// </summary>
    /// <param name="data">Строка в виде Json.</param>
    /// <returns>Структура с информацией о странице.</returns>
    [Public]
    public IPageInfo DeserializePageStruct(string jsonData)
    {
      if (string.IsNullOrEmpty(jsonData))
        return null;
      
      return JsonConvert.DeserializeObject<PageInfo>(jsonData);
    }
  }
}