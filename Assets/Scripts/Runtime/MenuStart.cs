using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class MenuStart : MonoBehaviour
{
    [SerializeField] private Curve _transitionCurve;
    [SerializeField] private AnimationCurve _transitionAnimationCurve;
    [SerializeField] private float _transitionDuration;
    [SerializeField, Range(0f, 1f)] private float _turnCurveTransitionPercentage = 0.6f;
    [SerializeField, Range(0f, 1f)] private float _blinkCurveTransitionPercentage = .95f;
    [SerializeField, Range(0f, 1f)] private float _rollCurveTransitionPercentage = .8f;
    [SerializeField] private float _rollForce = -90;
    [SerializeField] private float _transitionDamping;
    [SerializeField] private float _blinkDuration = 0.5f;

    [Header("Camera")]
    [SerializeField] private Camera _menuCamera;
    [SerializeField] private Camera _mainCamera;

    [Header("Canvas")] 
    [SerializeField] private CanvasGroup _titleCanva;

    private bool _hasBlinked;

    private void Start()
    {
        _mainCamera.gameObject.SetActive(false);
        _menuCamera.transform.position = _transitionCurve.GetPosition(0f, transform.localToWorldMatrix);
    }

    [Button]
    private void Transition()
    {
        _hasBlinked = false;
        _mainCamera.gameObject.SetActive(false);
        _menuCamera.gameObject.SetActive(true);
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        float timer = 0f;
        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            float time = _transitionAnimationCurve.Evaluate(timer / _transitionDuration);
            Vector3 targetPosition = _transitionCurve.GetPosition(time, transform.localToWorldMatrix);
            Vector3 nextTargetPosition = _transitionCurve.GetPosition(_transitionAnimationCurve.Evaluate(timer + Time.deltaTime / _transitionDuration), transform.localToWorldMatrix);
            _menuCamera.transform.position = _transitionCurve.GetPosition(time, transform.localToWorldMatrix);
            _menuCamera.fieldOfView = Mathf.Lerp(_menuCamera.fieldOfView, _mainCamera.fieldOfView, time);
            if (time < _turnCurveTransitionPercentage)
            {
                _menuCamera.transform.rotation = Quaternion.RotateTowards(_menuCamera.transform.rotation, Quaternion.LookRotation(nextTargetPosition - targetPosition), _transitionDamping);
            }
            else
            {
                _menuCamera.transform.rotation = Quaternion.RotateTowards(_menuCamera.transform.rotation, Quaternion.LookRotation(Vector3.right), _transitionDamping);
                
            }
            
            if(time > _rollCurveTransitionPercentage)
            {
                _menuCamera.transform.eulerAngles = Vector3.Lerp(_menuCamera.transform.eulerAngles, new Vector3(_menuCamera.transform.eulerAngles.x, _menuCamera.transform.eulerAngles.y, _rollForce), Time.deltaTime);
            }
            
            if (time > _blinkCurveTransitionPercentage)
            {
                if (!_hasBlinked)
                {
                    CanvasController.instance.StartFade(_blinkDuration);
                    _hasBlinked = true;
                }
            }
            yield return null;
        }
        _titleCanva.alpha = 0;
        _menuCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);
    }
    
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        _transitionCurve.DrawGizmo(Color.red, transform.localToWorldMatrix, Selection.activeGameObject == gameObject, 0.01f);
        #endif
    }
}
