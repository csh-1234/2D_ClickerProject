using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManagement
{
    private Dictionary<string, BuffInfo> _activeBuffs = new Dictionary<string, BuffInfo>();

    public class BuffInfo
    {
        public string BuffId { get; set; } 
        public Stats BuffStats { get; set; }
        public float EndTime { get; set; }
    }

    public void ApplyBuff(Creature target, string buffId, Stats buffStats, float duration)
    {
        // �̹� ���� ������ �ִٸ� ����
        if (_activeBuffs.TryGetValue(buffId, out BuffInfo existingBuff))
        {
            target.ApplyStatModifier(existingBuff.BuffStats, false);  // ���� ���� ����
            _activeBuffs.Remove(buffId);
        }

        // �� ���� ����
        var newBuff = new BuffInfo
        {
            BuffId = buffId,
            BuffStats = buffStats,
            EndTime = Time.time + duration
        };

        _activeBuffs.Add(buffId, newBuff);
        target.ApplyStatModifier(buffStats, true);
        if (target is Player player)
        {
            Managers.Instance.Game.UpdatePlayerStats();
        }
    }



    public void RemoveBuff(Creature target, string buffId)
    {
        if (_activeBuffs.TryGetValue(buffId, out BuffInfo buff))
        {
            target.ApplyStatModifier(buff.BuffStats, false);
            _activeBuffs.Remove(buffId);
        }
        if (target is Player player)
        {
            //Managers.Instance.Game.UpdatePlayerStats();
        }
    }
}
