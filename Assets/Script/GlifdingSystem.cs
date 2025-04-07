using UnityEngine;
using DG.Tweening;

/// <summary>
/// Système de planeur indépendant qui peut être ajouté séparément du PlayerController
/// </summary>
public class GliderSystem : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private InputManager inputManager;  // Référence vers votre InputManager
    [SerializeField] private Rigidbody playerRigidbody;  // Le Rigidbody du joueur
    [SerializeField] private Transform propellerBase;    // Objet parent du propulseur/parachute
    [SerializeField] private Transform propellers;       // Les hélices qui tournent
    [SerializeField] private Animator animator;          // Référence à l'Animator du joueur (optionnel)
    [SerializeField] private LayerMask groundLayer;      // Layer pour la détection du sol

    [Header("Paramètres de base")]
    [SerializeField] private float glidingSpeed = 10f;           // Vitesse de vol plané
    [SerializeField] private float glidingGravity = -2f;         // Gravité réduite pendant le vol plané
    [SerializeField] private float normalGravity = -9.8f;        // Gravité normale
    [SerializeField] private float glidingDrag = 3.5f;           // Résistance à l'air en vol plané
    [SerializeField] private float normalDrag = 0.5f;            // Résistance à l'air normale

    [Header("Contrôle vertical")]
    [SerializeField] private float ascentForce = 5f;             // Force pour monter
    [SerializeField] private float descentForce = 3f;            // Force pour descendre
    [SerializeField] private float maxAscentAngle = 25f;         // Angle max de montée
    [SerializeField] private float maxDescentAngle = 35f;        // Angle max de descente
    [SerializeField] private float glideTiltSpeed = 3f;          // Vitesse d'inclinaison
    [SerializeField] private float turnMultiplier = 1.2f;        // Multiplicateur de virage

    // Variables privées
    private bool isGliding = false;
    private bool isGrounded = true;
    private float timeInAir = 0f;
    private Vector3 initialPropellerScale;
    private float targetPitchAngle = 0f;
    private float smoothedVerticalInput = 0f;
    private float originalLinearDamping;
    private Vector3 originalGravity;

    private void Start()
    {
        // Stocker les valeurs initiales
        if (propellerBase != null)
        {
            initialPropellerScale = propellerBase.localScale;
            propellerBase.gameObject.SetActive(false);
        }

        // Stocker les paramètres physiques originaux
        if (playerRigidbody != null)
        {
            originalLinearDamping = playerRigidbody.linearDamping;
            originalGravity = Physics.gravity;
        }

        // Si inputManager n'est pas assigné, essayer de le trouver automatiquement
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        // Si playerRigidbody n'est pas assigné, essayer de le trouver automatiquement
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // Si animator n'est pas assigné, essayer de le trouver automatiquement
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        CheckGround();

        // Gérer l'activation/désactivation du vol plané
        if (inputManager.isGliding && !isGrounded && CanDeployGlider())
        {
            if (!isGliding)
            {
                StartGliding();
            }
        }
        else if (isGliding && (isGrounded || !inputManager.isGliding))
        {
            StopGliding();
        }

        // Si nous sommes en train de planer, gérer les contrôles
        if (isGliding)
        {
            HandleGliding();
        }
    }

    private bool CanDeployGlider()
    {
        // Vérifier si le joueur est en l'air depuis assez longtemps
        return timeInAir > 0.5f && !IsTooCloseToGround();
    }

    private bool IsTooCloseToGround()
    {
        RaycastHit hit;
        float minimumHeight = 5f; // Hauteur minimale pour déployer
        return Physics.Raycast(transform.position, Vector3.down, out hit, minimumHeight, groundLayer);
    }

    private void CheckGround()
    {
        RaycastHit hit;
        float checkDistance = 0.6f;

        isGrounded = Physics.Raycast(
            transform.position + transform.up * 0.3f,
            -transform.up,
            out hit,
            checkDistance,
            groundLayer
        );

        if (isGrounded)
        {
            timeInAir = 0f;
            if (isGliding)
            {
                StopGliding();
            }
        }
        else
        {
            timeInAir += Time.deltaTime;
        }
    }

    private void StartGliding()
    {
        isGliding = true;

        // Réduire la vitesse de chute pour une transition plus douce
        if (playerRigidbody != null)
        {
            Vector3 currentVel = playerRigidbody.linearVelocity;
            if (currentVel.y < 0)
            {
                playerRigidbody.linearVelocity = new Vector3(currentVel.x, currentVel.y * 0.5f, currentVel.z);
            }

            // Appliquer la résistance à l'air du planeur
            playerRigidbody.linearDamping = glidingDrag;
        }

        // Déployer le propulseur/parachute
        if (propellerBase != null)
        {
            propellerBase.gameObject.SetActive(true);
            propellerBase.localScale = Vector3.zero;

            // Animation de l'apparition du propulseur
            propellerBase.DOScale(initialPropellerScale, 0.3f)
                .SetEase(Ease.OutElastic);

            // Rotation des hélices
            if (propellers != null)
            {
                propellers.DOLocalRotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360)
                    .SetRelative(true)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1); // Boucle infinie
            }
        }

        // Mettre à jour l'animator si disponible
        if (animator != null)
        {
            animator.SetBool("Gliding", true);
            animator.SetBool("flying", true);
        }
    }

    private void StopGliding()
    {
        isGliding = false;

        // Réinitialiser la résistance à l'air
        if (playerRigidbody != null)
        {
            playerRigidbody.linearDamping = originalLinearDamping;
        }

        // Rétracter le propulseur/parachute
        if (propellerBase != null)
        {
            propellerBase.DOKill();
            if (propellers != null) propellers.DOKill();

            // Animation de disparition
            propellerBase.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                propellerBase.gameObject.SetActive(false);
            });
        }

        // Mettre à jour l'animator
        if (animator != null)
        {
            animator.SetBool("Gliding", false);
            animator.SetBool("flying", false);
        }
    }

    private void HandleGliding()
    {
        // Obtenir les entrées
        float horizontalInput = inputManager.move.x;
        float forwardInput = Mathf.Max(0.3f, inputManager.move.y); // Toujours avoir un minimum d'avancée

        // Pour la démo, utilisez l'axe vertical existant pour le contrôle de montée/descente
        // En production, il faudrait ajouter un axe dédié dans l'InputManager
        float verticalInput = 0f;

        // Si vous avez ajouté verticalControl à votre InputManager, utilisez:
        if (inputManager.GetType().GetField("verticalControl") != null)
        {
            // Utiliser la réflexion pour obtenir la valeur si elle existe
            verticalInput = (float)inputManager.GetType().GetField("verticalControl").GetValue(inputManager);
        }

        // Lisser l'entrée verticale
        smoothedVerticalInput = Mathf.Lerp(smoothedVerticalInput, verticalInput, Time.deltaTime * 3f);

        // Appliquer les forces selon l'entrée verticale
        if (playerRigidbody != null)
        {
            // Appliquer la gravité de base du planeur
            playerRigidbody.AddForce(Vector3.up * glidingGravity, ForceMode.Acceleration);

            if (smoothedVerticalInput > 0.1f)
            {
                // Montée
                float upForce = smoothedVerticalInput * ascentForce;
                playerRigidbody.AddForce(transform.up * upForce, ForceMode.Acceleration);

                // Ajuster l'angle de tangage pour la montée
                targetPitchAngle = -smoothedVerticalInput * maxAscentAngle;
            }
            else if (smoothedVerticalInput < -0.1f)
            {
                // Descente
                float downForce = -smoothedVerticalInput * descentForce;
                playerRigidbody.AddForce(-transform.up * downForce, ForceMode.Acceleration);

                // Ajuster l'angle de tangage pour la descente
                targetPitchAngle = -smoothedVerticalInput * maxDescentAngle;
            }
            else
            {
                // Vol horizontal
                targetPitchAngle = 0f;
            }
        }

        // Déplacement avant
        transform.position += transform.forward * forwardInput * glidingSpeed * Time.deltaTime;

        // Rotation (virage)
        float turnAmount = horizontalInput * turnMultiplier * Time.deltaTime * 90f;
        transform.Rotate(0, turnAmount, 0);

        // Appliquer l'angle de tangage avec interpolation
        Vector3 currentEuler = transform.eulerAngles;
        float currentPitch = currentEuler.x;
        if (currentPitch > 180f) currentPitch -= 360f;

        float newPitch = Mathf.Lerp(currentPitch, targetPitchAngle, Time.deltaTime * glideTiltSpeed);
        transform.rotation = Quaternion.Euler(newPitch, currentEuler.y, currentEuler.z);

        // Mettre à jour l'animator si disponible
        if (animator != null && animator.parameters.Length > 0)
        {
            // Vérifier si le paramètre existe avant de l'utiliser
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == "GlideVertical")
                {
                    animator.SetFloat("GlideVertical", smoothedVerticalInput);
                    break;
                }
            }
        }
    }

    // Méthode publique pour activer/désactiver manuellement le mode planeur
    public void ToggleGliding()
    {
        if (isGliding)
        {
            StopGliding();
        }
        else if (!isGrounded && CanDeployGlider())
        {
            StartGliding();
        }
    }
}