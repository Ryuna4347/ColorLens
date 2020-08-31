using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterMoveGuide : MonoBehaviour
{
	public Text txtMove;

	public Transform holder;

	public CanvasGroup cg;

	Sequence seq = null;

	private void Awake()
	{
		cg.alpha = 0f;
	}

	public void Show(int moveCount)
	{
		string moveWord = moveCount.ToString();
		if( moveCount > 10 ) {		//흰색인경우.
			moveWord = "끝까지 직진";
		} else {
			moveWord = string.Format( "{0}칸 이동!", moveWord );
		}

		Rewind();
		seq = DOTween.Sequence();
		seq.Append( holder.DOScale( 1f, 0.05f ).SetEase( Ease.OutBack ) );
		seq.Append( txtMove.DOText( moveWord, 0.5f ) );
		seq.AppendInterval( 1f );
		seq.Append( cg.DOFade(0f, 1f) ).SetEase(Ease.InCirc);
		seq.OnComplete( () => Rewind() );
		seq.Play();
	}

	public void Rewind()
	{
		if ( seq != null )
			seq.Kill( true );

		txtMove.text = string.Empty;
		holder.localScale = Vector3.zero;
		cg.alpha = 1f;
	}
}
