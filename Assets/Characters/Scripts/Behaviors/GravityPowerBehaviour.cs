using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RunBehaviour))]
public class GravityPowerBehaviour : PlayerBehaviour
{
    public override void Init()
    {
        base.Init();

        player.actionManager.AddAction("Gravity", context =>
        {
            TryStartGravityTransition();
        });
    }

    public override void Run()
    {
        // base.Run(); Do Nothing
    }

    void Update()
    {
        // Si en cours d'animation
        if (player.isAnimated)
            return;

        // Si la gravité est aligné vers le sol
        if (Vector3.Angle(transform.up, Vector3.up) < 1f)
            return;
        
        // Si il y a un wall sous les pieds, on oriente la gravité vers le mur
        float maxDistance = 6f;
        Vector3 origin = transform.position + transform.up * 0.7f;
        Vector3 direction = -transform.up;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.blue, 1f);

            Wall wall = hit.collider.GetComponent<Wall>();
            if (wall != null && wall.isWalkable)
            {
                Debug.Log($"GravityPower wall ${wall.name}");
                TryRotateToNormal(hit.normal);
                return;
            }
        }

        // On realigne la gravité vers le sol
        TryRotateToNormal(Vector3.up);
    }

    bool TryStartGravityTransition()
    {
        Debug.Log("StartGravityTransition");
        Vector3 origin = transform.position + transform.up * 0.7f;
        Vector3 direction = transform.forward;

        if (TryRotateToWallNormal(origin, direction))
            return true;

        Debug.Log("GravityPower no wall");
        return false;
    }

    private bool TryRotateToWallNormal(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        float maxDistance = 1.8f;

        // Debug.DrawRay(origin, direction * maxDistance, Color.red, 1f);
        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.green, 1f);

            Wall wall = hit.collider.GetComponent<Wall>();
            
            if (wall != null && wall.isWalkable)
            {
                Debug.Log($"GravityPower wall ${wall.name}");
                return TryRotateToNormal(hit.normal);
            }

            Debug.Log($"GravityPower no wall");
        }

        return false;
    }

    private bool TryRotateToNormal(Vector3 normal)
    {
        if (player.isAnimated)
            return false;

        if (Vector3.Angle(transform.up, normal) < 1f)
            return false;
        
        player.isAnimated = true;
        player.ChangeBehaviour(this);
        player.animator.SetBool("GravityPower", true);

        Quaternion endRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        Debug.Log($"TryRotateToNormal ${endRotation} Begin");
        transform.DORotateQuaternion(endRotation, 1f).AsyncWaitForCompletion().GetAwaiter().OnCompleted(() =>
        {
            player.ChangeBehaviour<RunBehaviour>();
            player.isAnimated = false;
            player.animator.SetBool("GravityPower", false);
            Debug.Log($"TryRotateToNormal ${endRotation} End");
        });

        return true;
    }
}