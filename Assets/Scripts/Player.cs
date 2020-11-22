using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour{

    [SerializeField] private float speed;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private AudioClip engineSound;
    [SerializeField] private AudioClip skidSound;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private AudioClip finishSound;

    
    private enum State{
        Start, Driving, Dying
    }
    
    private Rigidbody2D _rigidbody2D;
    private FixedJoint2D _fixedJoint2D;
    private State _state = State.Start;
    private AudioSource _audioSource;
    private bool _isSkidding = false;
    private int _scene;
    private bool _isSlinging;
    private GameObject _nearestSlingBase;
    
    private LevelManager _levelManager;
    private UIManager _uiManager;
    private GameObject uiManagerObject;
    private GameObject levelManagerObject;
    // Start is called before the first frame update
    private void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _fixedJoint2D = GetComponent <FixedJoint2D>();
        _scene = SceneManager.GetActiveScene().buildIndex;
        _audioSource = GetComponent<AudioSource>();

        levelManagerObject = GameObject.FindWithTag("LevelManager");
        if(levelManagerObject != null)
            _levelManager  = levelManagerObject.GetComponent<LevelManager>();
        
        uiManagerObject = GameObject.FindWithTag("UIManager");
        if (uiManagerObject != null) {
            _uiManager = uiManagerObject.GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && _state == State.Start) {
            _state = State.Driving;
            _uiManager.showHidePlayText(false);
        }

        if (!_audioSource.isPlaying) {
            if (_isSkidding)
                _audioSource.PlayOneShot(skidSound);
            else
                _audioSource.PlayOneShot(engineSound, 0.5f);
        }

        if (_state == State.Driving)
            ProcessMouseInput(transform.position);

        if (_isSlinging) {
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _nearestSlingBase.transform.position);
            _fixedJoint2D.connectedBody = _nearestSlingBase.GetComponent<Rigidbody2D>();
            _fixedJoint2D.enabled = true;
            _rigidbody2D.freezeRotation = false;
        }
        else {
            _fixedJoint2D.enabled = false;
            _rigidbody2D.freezeRotation = true;
            _isSkidding = false;
            lineRenderer.SetVertexCount(0);
        }
    }

   
    private void ProcessMouseInput(Vector3 carPosition) {
        
        GameObject nearestSlingBaseObject = _levelManager.GetNearestSlingBase(transform.position);
        if (Input.GetMouseButton(0)) {
            if (nearestSlingBaseObject != null) {
                _nearestSlingBase = nearestSlingBaseObject;
                _isSlinging = true;
            }
        }
        else {
            _isSlinging = false;
        }
    }
    
    
    private void FixedUpdate() {
        if (_state == State.Driving)
            _rigidbody2D.velocity = speed * transform.up;
        else if (_state == State.Dying)
            _rigidbody2D.velocity = Vector2.zero;
    }

    
    private void LoadCurrentLevel() {
        _uiManager.showHidePlayText(true);
        _uiManager.showHideLoseScreen(false);
        SceneManager.LoadScene(_scene);
    }

    private void LoadNextScene() {
        int nextScene = _scene == 4 ? 0 : _scene+1;
        PlayerPrefs.SetInt("scene", nextScene);
        SceneManager.LoadScene(nextScene);
    }
    
    private void StartDeathSequence() {
        _uiManager.showHideLoseScreen(true);
        _state = State.Dying;
        _audioSource.Stop();
        _audioSource.PlayOneShot(crashSound);
        Invoke(nameof(LoadCurrentLevel), 2f);
    }

    private void StartSuccessSequence() {
        _audioSource.Stop();
        _audioSource.PlayOneShot(finishSound);
        Invoke(nameof(LoadNextScene), 1f);
    }

    
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Finish")) {
            StartSuccessSequence();
        } else if (other.gameObject.CompareTag("Track")) {
            StartDeathSequence();
        }
    }
}
