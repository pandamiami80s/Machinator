using UnityEngine;

public class TurretSlot : MonoBehaviour
{
    [Header("Объекты")]
    public Transform horizontalPivot; // Поворот Y (влево/вправо)
    public Transform verticalPivot;   // Поворот X (вверх/вниз)
    public Weapon currentWeapon;      // Скрипт оружия

    [Header("Ограничения углов")]
    public bool useLimits = true;
    public float minHorizontalAngle = -45f;
    public float maxHorizontalAngle = 45f;
    public float minVerticalAngle = -10f;
    public float maxVerticalAngle = 30f;

    [Header("Настройки")]
    public float rotationSpeed = 5f;

    private float _currentY;
    private float _currentX;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void AimAt(Vector3 targetPoint)
    {
        // --- 1. ГОРИЗОНТАЛЬ (Y-Axis) ---
        // Вычисляем направление от основания до цели
        Vector3 dirToTarget = targetPoint - horizontalPivot.position;

        // Переводим направление в локальное пространство РОДИТЕЛЯ турели (кузова машины)
        // Это критически важно, чтобы турель не крутилась вместе с машиной, а держала цель
        Vector3 localDir = transform.parent.InverseTransformDirection(dirToTarget);
        localDir.y = 0; // Нас интересует только плоскость горизонта

        if (localDir != Vector3.zero)
        {
            // Находим целевой локальный поворот
            Quaternion targetRotationY = Quaternion.LookRotation(localDir);

            // Плавный поворот через Slerp (как в вашем Update)
            horizontalPivot.localRotation = Quaternion.Slerp(
                horizontalPivot.localRotation,
                targetRotationY,
                Time.deltaTime * rotationSpeed
            );

            // Если нужны жесткие ограничения (Clamp) для Y:
            if (useLimits)
            {
                Vector3 euler = horizontalPivot.localEulerAngles;
                float angleY = NormalizeAngle(euler.y);
                angleY = Mathf.Clamp(angleY, minHorizontalAngle, maxHorizontalAngle);
                horizontalPivot.localRotation = Quaternion.Euler(0, angleY, 0);
            }
        }

        // --- 2. ВЕРТИКАЛЬ (X-Axis) ---
        // Используем InverseTransformPoint для получения точных локальных координат цели относительно основания
        Vector3 localTargetPos = horizontalPivot.InverseTransformPoint(targetPoint);

        // Вычисляем угол наклона через арктангенс (самый точный способ)
        float targetX = -Mathf.Atan2(localTargetPos.y, localTargetPos.z) * Mathf.Rad2Deg;

        // Ограничиваем угол
        if (useLimits)
        {
            targetX = Mathf.Clamp(targetX, minVerticalAngle, maxVerticalAngle);
        }

        // Плавный наклон ствола
        Quaternion targetRotationX = Quaternion.Euler(targetX, 0, 0);
        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            targetRotationX,
            Time.deltaTime * rotationSpeed
        );
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}