using System;
using UnityEngine;
using UnityEngine.UI;

namespace Examples
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Animator))] // Unable to use the component without specified one
    public class MyAttributes : MonoBehaviour
    {
        [Header("Example header")] // Header of the field
        public string text;
        [Space] // Adds space between two fields
        public Color color;

        [SerializeField] // private field but inspector see it
        private int _speed;

        [HideInInspector] // public field but hidden for inspector
        public bool hideInInspector;
        
        private void Update()
        {
            print("Executing everywhere!");
        }
    }
}