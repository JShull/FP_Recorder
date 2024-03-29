using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    public interface IRecorderOutputFormat<T,A>
    {
        public T ReturnUnityOutputFormat(A inputParameters);
    }
}
