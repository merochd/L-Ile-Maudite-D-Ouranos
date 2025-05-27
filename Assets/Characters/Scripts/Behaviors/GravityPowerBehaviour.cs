using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(RunBehaviour))]
public class GravityPowerBehaviour : PlayerBehaviour
{
    public override void Init()
    {
        actionManager.AddAction("Gravity", context =>
        {
            if (player.currentBehaviour != this)
            {
                player.ChangeBehaviour(this);
            }
        });
    }

    public override void Enter()
    {
        GravityPower();
        player.ChangeBehaviour<RunBehaviour>();
        animator.SetBool("GravityPower", true);
    }

    public override void Exit()
    {
        animator.SetBool("GravityPower", false);
    }

    public override void Run()
    {
    }

    public void GravityPower()
    {
        Vector3 origin = transform.position + transform.up * 0.7f;
        Vector3 direction = (transform.forward - transform.up).normalized;

        if (GravityPowerRaycast(origin, direction))
        {
            return;
        }
    }

    public bool GravityPowerRaycast(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        float maxDistance = 1.8f;

        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            Wall wall = hit.collider.GetComponent<Wall>();
            if (wall != null && !wall.isWalkable)
            {
                return false;
            }

            DoRotateToNormal(hit.normal); // Rotation vers la surface touchée
            return true;
        }
        else
        {
            ReturnToWorldRotation(); // Revenir à la rotation du monde s'il n'y a rien
        }

        return false;
    }

    private void DoRotateToNormal(Vector3 normal)
    {
        if (Vector3.Angle(transform.up, normal) < 1f)
            return;

        Quaternion endRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        transform.DORotateQuaternion(endRotation, 1f);
    }

    public void ReturnToWorldRotation()
    {
        DoRotateToNormal(Vector3.up); // Revenir à l'orientation du monde
    }
}