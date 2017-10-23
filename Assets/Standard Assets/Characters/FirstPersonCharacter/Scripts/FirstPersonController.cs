using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;

        public Rigidbody character;
        private Camera m_Camera;
        private Vector3 m_OriginalCameraPosition;

        // Use this for initialization
        private void Start()
        {
            m_Camera = GetComponentInChildren<Camera>();
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
			m_MouseLook.Init(transform , m_Camera.transform);
            character = GetComponent<Rigidbody>();
        }


        // Update is called once per frame
        private void Update()
        {
            //Debug.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * 30, Color.red);
            RotateView();
        }

        private void FixedUpdate()
        {
            m_MouseLook.UpdateCursorLock();
            if (Input.GetKey(KeyCode.W) && character.velocity == Vector3.zero)
            {
                character.isKinematic = false;
                character.AddForce(m_Camera.transform.forward * 500);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            character.isKinematic = true;
            character.velocity = Vector3.zero;
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }
    }
}
