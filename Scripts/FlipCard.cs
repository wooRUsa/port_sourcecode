using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(BoxCollider2D))]
public class FlipCard : MonoBehaviour
{
    static readonly Vector3 InvisibleScale = new Vector3(0, 1, 1);


    [SerializeField] float _speed = 1;
    [SerializeField] GameObject _face = null, _back = null;

    bool _mouseOver;


    Vector3 ScaleChange => new Vector3(Time.deltaTime * _speed, 0, 0);


    void OnValidate()
    {
        _face.SetActive(true);
        _back.SetActive(false);
        _back.transform.localScale = InvisibleScale;
    }

    void OnMouseEnter() => _mouseOver = true;

    void OnMouseOver() => Flip(_face, _back);

    void OnMouseExit() => _mouseOver = false;

    void Update()
    {
        if (!_mouseOver) Flip(_back, _face);
    }

    void Flip(GameObject from, GameObject to)
    {
        if (from.activeSelf)
        {
            from.transform.localScale -= ScaleChange;
            if (from.transform.localScale.x < 0)
            {
                from.transform.localScale = InvisibleScale;
                from.SetActive(false);
                to.SetActive(true);
            }
            return;
        }
        if (to.transform.localScale.x < 1)
        {
            to.transform.localScale += ScaleChange;
            return;
        }
        to.transform.localScale = Vector3.one;
    }
}
