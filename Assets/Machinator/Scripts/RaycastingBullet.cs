using UnityEngine;

public class RaycastingBullet : MonoBehaviour
{
    [Header("Настройки полета")]
    public float speed = 200f;        // Скорость пули
    public int damage = 20;        // Урон
    public float maxDistance = 500f;  // Максимальная дистанция полета
    public LayerMask hitLayers;       // Слои, в которые пуля может попасть

    [Header("Визуализация")]
    public GameObject hitEffectPrefab; // Искры/взрыв при попадании
    public float lifeTime = 3f;        // Время жизни объекта, если никуда не попал

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
        // Уничтожаем пулю по истечении времени жизни (безопасность)
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 1. Рассчитываем дистанцию, которую пуля должна пролететь в этом кадре
        float stepDistance = speed * Time.deltaTime;

        // 2. Пускаем луч вперед на расстояние одного шага
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, stepDistance, hitLayers))
        {
            HandleHit(hit);
        }
        else
        {
            // 3. Если препятствий нет — просто двигаем объект пули вперед
            transform.position += transform.forward * stepDistance;
        }

        // 4. Проверка на максимальную дистанцию (чтобы не летела вечно)
        if (Vector3.Distance(_startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        // Перемещаем пулю точно в точку попадания для визуального эффекта
        transform.position = hit.point;

        // Наносим урон
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

        // Создаем эффект искр/дырок от пуль
        if (hitEffectPrefab != null)
        {
            // Поворачиваем эффект «лицом» к нормали поверхности
            Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }

        // Лог для дебага (можно удалить)
        Debug.Log($"Попал в {hit.collider.name}");

        // Уничтожаем объект пули
        Destroy(gameObject);
    }
}