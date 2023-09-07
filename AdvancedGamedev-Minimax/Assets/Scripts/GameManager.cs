using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Public Variables

    public CanvasGroup selectionCanvas = null;

    public Dice dice = null;

    public float fadeTime = 0.0f;

    public GameBoard gameBoard = null;

    public RectTransform orangeStartsText = null;
    public RectTransform blueStartsText = null;

    public RectTransform orangeWinsText = null;
    public RectTransform blueWinsText = null;

    public float textStartPosition;
    public float textEndPosition;

    #endregion

    #region Private Variables

    private GameState gameState;

    #endregion

    public GameState GameState => this.gameState;

    void Start()
    {
        this.gameState = GameState.SELECT_PLAYMODE;
    }

    public IEnumerator Win(PlayerColor winningColor)
    {
        this.gameState = GameState.SELECT_PLAYMODE;

        var animatedTransform = winningColor == PlayerColor.ORANGE ? this.orangeWinsText : this.blueWinsText;
        animatedTransform.gameObject.SetActive(true);

        var centerPosition = animatedTransform.anchoredPosition;
        float startY = centerPosition.y;
        float centerX = animatedTransform.position.x;
        animatedTransform.anchoredPosition = new Vector2(this.textStartPosition, startY);

        var moveSequence = DOTween.Sequence();

        var moveToCenter = animatedTransform.DOMoveX(centerX, 1.0f);
        moveToCenter.SetEase(Ease.OutExpo);

        var moveFromCenter = animatedTransform.DOMoveX(this.textEndPosition, 1.0f);
        moveFromCenter.SetEase(Ease.InCubic);

        moveSequence.Append(moveToCenter);
        moveSequence.AppendInterval(0.5f);
        moveSequence.Append(moveFromCenter);

        moveSequence.Play();

        yield return moveSequence.WaitForCompletion();

        animatedTransform.gameObject.SetActive(false);
        animatedTransform.anchoredPosition = centerPosition;

        yield return this.FadeInSelectionCanvas();
    }

    private IEnumerator FadeInSelectionCanvas()
    {
        this.selectionCanvas.gameObject.SetActive(true);
        this.selectionCanvas.alpha = 0.0f;

        float timer = 0.0f;
        while(timer < this.fadeTime)
        {
            float percent = timer / this.fadeTime;

            this.selectionCanvas.alpha = percent;

            yield return null;

            timer += Time.deltaTime;
        }

        this.selectionCanvas.alpha = 1.0f;
    }

    private IEnumerator FadeOutSelectionCanvas()
    {
        float timer = 0.0f;
        while(timer < this.fadeTime)
        {
            float percent = 1.0f - (timer / this.fadeTime);

            this.selectionCanvas.alpha = percent;

            yield return null;

            timer += Time.deltaTime;
        }

        this.selectionCanvas.alpha = 0.0f;
        this.selectionCanvas.gameObject.SetActive(false);
    }

    public void StartOneOnOneGame()
    {
        this.StartCoroutine(this.StartOneOnOneGameRoutine());
    }

    private IEnumerator StartOneOnOneGameRoutine()
    {
        yield return this.StartCoroutine(this.FadeOutSelectionCanvas());
        yield return this.StartCoroutine(this.Roll());
    }

    public IEnumerator Roll()
    {
        yield return this.FadeOutSelectionCanvas();

        this.gameState = GameState.ROLL_DICE;

        bool isRolling = true;
        this.gameBoard.ResetGame();
        this.dice.gameObject.SetActive(true);
        this.dice.StartCoroutine(this.dice.Roll());

        while(isRolling)
        {
            yield return null;

            isRolling = !this.dice.RollIsFinished();
        }

        var color = this.dice.GetDiceColor();

        var animatedTransform = color == Dice.DiceColor.BLUE ? this.blueStartsText : this.orangeStartsText;
        animatedTransform.gameObject.SetActive(true);

        var centerPosition = animatedTransform.anchoredPosition;
        float startY = centerPosition.y;
        float centerX = animatedTransform.position.x;
        animatedTransform.anchoredPosition = new Vector2(this.textStartPosition, startY);

        var moveSequence = DOTween.Sequence();

        
        var moveToCenter = animatedTransform.DOMoveX(centerX, 1.0f);
        moveToCenter.SetEase(Ease.OutExpo);

        var moveFromCenter = animatedTransform.DOMoveX(this.textEndPosition, 1.0f);
        moveFromCenter.SetEase(Ease.InCubic);

        moveSequence.Append(moveToCenter);
        moveSequence.AppendInterval(0.5f);
        moveSequence.Append(moveFromCenter);

        moveSequence.Play();

        yield return new WaitForSeconds(1.5f);

        this.dice.StartCoroutine(this.dice.Vanish());

        yield return moveSequence.WaitForCompletion();

        animatedTransform.gameObject.SetActive(false);
        animatedTransform.anchoredPosition = centerPosition;

        this.gameState = GameState.GAME;
        this.gameBoard.SetStartColor(color == Dice.DiceColor.BLUE ? PlayerColor.BLUE : PlayerColor.ORANGE);
    }

    void Update()
    {
        
    }
}
