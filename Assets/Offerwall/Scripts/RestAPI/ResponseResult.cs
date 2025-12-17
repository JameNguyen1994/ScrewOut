using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rest.API
{
    public class ResponseResult<T>
    {
        public T Data { get; set; }
        public string Error { get; set; }
    }
}
