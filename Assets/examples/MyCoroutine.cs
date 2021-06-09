using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{
    public class MyCoroutine : MonoBehaviour
    {
        private int _ticks;

        private Coroutine _coroutine;
        // Start is called before the first frame update
        void Start()
        {
            _ticks = 0;
        }

        private IEnumerator Timer()
        {
            while (true)
            {
                print(_ticks.ToString());
                _ticks++;
                yield return new WaitForSeconds(1);
                // yield return break -> stop coroutine
                // yield return null -> wail until next frame
                // yield return WaitUntil(() => bool) -> wail until the function return true
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(Timer());
            }

            if (_ticks >= 5)
            {
                StopCoroutine(_coroutine);
            }
        }
    }
}
