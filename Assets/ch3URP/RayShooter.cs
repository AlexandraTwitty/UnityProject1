using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RayShooter : MonoBehaviour {
    private Camera _camera;
    [SerializeField] private GameObject reticle;
    
    [Header("Bullet Visual Effects")]
    [SerializeField] private LineRenderer bulletTrail;
    [SerializeField] private float trailDuration = 0.1f;
    [SerializeField] private Material bulletMaterial;
    [SerializeField] private float trailStartDistance = 1f;
    
    [Header("Bullet Colors")]
    [SerializeField] private Color bulletStartColor = Color.yellow;
    [SerializeField] private Color bulletEndColor = Color.white;
    [SerializeField] private float bulletWidth = 0.08f;
    
    [Header("Impact Indicator")]
    [SerializeField] private Material sphereMaterial;
    [SerializeField] private Color sphereColor = Color.red;
    [SerializeField] private float sphereSize = 0.2f;
    [SerializeField] private float sphereDuration = 1f;
    [SerializeField] private bool sphereGlow = false;
    
    [Header("Optional Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    
    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    void Start() {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        reticle = GameObject.Find("Reticle");
        if (reticle != null) {
            reticle.GetComponent<Text>().text = "+";
            reticle.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            reticle.GetComponent<RectTransform>().position =
                new Vector3(_camera.pixelWidth / 2.0f,
                            _camera.pixelHeight / 2.0f,
                            0.0f);
        }

        SetupBulletTrail();
        SetupAudio();
    }

    void SetupBulletTrail() {
        if (bulletTrail == null) {
            GameObject trailObj = new GameObject("BulletTrail");
            trailObj.transform.SetParent(transform, false);
            bulletTrail = trailObj.AddComponent<LineRenderer>();
        }

        bulletTrail.useWorldSpace = true;
        bulletTrail.startWidth = bulletWidth;
        bulletTrail.endWidth = bulletWidth * 0.4f;
        bulletTrail.positionCount = 2;
        bulletTrail.enabled = false;
        bulletTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        bulletTrail.receiveShadows = false;
        bulletTrail.alignment = LineAlignment.View;
        
        if (bulletMaterial != null) {
            bulletTrail.material = bulletMaterial;
        } else {
            bulletTrail.material = new Material(Shader.Find("Sprites/Default"));
            
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(bulletStartColor, 0.0f), 
                    new GradientColorKey(bulletEndColor, 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(0.8f, 1.0f) 
                }
            );
            bulletTrail.colorGradient = gradient;
        }
    }

    void SetupAudio() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update() {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current.leftButton.wasPressedThisFrame) {
#else
        if (Input.GetMouseButtonDown(0)) {
#endif
            Shoot();
        }
    }

    void Shoot() {
        Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        
        Vector3 trailStart = ray.origin + ray.direction * trailStartDistance;
        Vector3 targetPoint;
        
        if (muzzleFlashPrefab != null) {
            GameObject flash = Instantiate(muzzleFlashPrefab, trailStart, _camera.transform.rotation);
            Destroy(flash, 0.2f);
        }
        
        if (shootSound != null && audioSource != null) {
            audioSource.PlayOneShot(shootSound);
        }
        
        if (Physics.Raycast(ray, out hit)) {
            targetPoint = hit.point;
            
            if (impactEffectPrefab != null) {
                GameObject impact = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1f);
            }
            
            GameObject hitObject = hit.transform.gameObject;
            ReactiveTarget target = hitObject.GetComponentInParent<ReactiveTarget>();
            if (target != null) {
                target.ReactToHit();
            } else {
                StartCoroutine(SphereIndicator(hit.point));
            }
        } else {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        StartCoroutine(ShowBulletTrail(trailStart, targetPoint));
    }

    private IEnumerator ShowBulletTrail(Vector3 startPoint, Vector3 endPoint) {
        bulletTrail.SetPosition(0, startPoint);
        bulletTrail.SetPosition(1, endPoint);
        bulletTrail.enabled = true;

        yield return new WaitForSeconds(trailDuration);

        bulletTrail.enabled = false;
    }

    private IEnumerator SphereIndicator(Vector3 pos) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.localScale = Vector3.one * sphereSize;
        
        // ⭐ Apply material or color
        Renderer renderer = sphere.GetComponent<Renderer>();
        
        if (sphereMaterial != null) {
            // Use custom material
            renderer.material = sphereMaterial;
        } else {
            // Create material with color
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = sphereColor;
            
            // Optional glow effect
            if (sphereGlow) {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", sphereColor * 2f);
            }
            
            renderer.material = mat;
        }

        yield return new WaitForSeconds(sphereDuration);

        Destroy(sphere);
    }
}