using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string _enemyName;
    [SerializeField] private string _description;
    [SerializeField] private int _cost;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _moveSpeed;

    public string ActorName { get { return _enemyName; } }
    public string Description { get { return _description; } }
    public int Cost { get { return _cost; } }
    public int Damage { get { return _damage; } }
    public float AttacDelay { get { return _attackDelay; } }
    public float MoveSpeed { get { return _moveSpeed; } }
}
