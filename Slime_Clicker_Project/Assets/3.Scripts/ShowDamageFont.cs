using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    TextMeshPro _damageText;

    public void SetInfo(Vector2 pos, float damage = 0, float healAmount = 0, Transform parent = null, bool isCritical = false)
    {
        _damageText = GetComponent<TextMeshPro>();
        transform.position = pos;

        if (isCritical)
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Color.white;
        }
        else
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Color.white;
        }
        _damageText.alpha = 1;
        if (parent != null)
        {
            GetComponent<MeshRenderer>().sortingOrder = 123;
        }
        DoAnimation();
    }
    private void DoAnimation()
    {
        Sequence seq = DOTween.Sequence();

        //작 -> 크 ->조금 작
        transform.localScale = new Vector3(0, 0, 0);
        seq.Append(transform.DOScale(0.5f, 0.1f).SetEase(Ease.InOutBounce))
            .Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
            .Append(transform.DOScale(0.5f, 0.1f).SetEase(Ease.InOutBounce))
            .Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
            .OnComplete(() =>
            {
                Managers.Instance.Resource.Destroy(gameObject);
            });

    }
}