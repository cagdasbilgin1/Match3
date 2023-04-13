using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


/// <summary>
/// Provides functionalities for a ScrollRect that snaps.
/// </summary>
/// <remarks></remarks>
[RequireComponent(typeof(ScrollRect))]
public class FAScrollSnapBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Public Properties

    /// <summary>
    /// Returns true when the Scroll View is moved true button, and the movement is still in action. 
    /// </summary>
    public bool IsLerping
    {
        get { return _lerp; }
    }
    /// <summary>
    /// Get the scroll view velocity (velocity != 0 only when scroll view is dragged, when button is clicked velocity == 0)
    /// </summary>
    public Vector2 Velocity
    {
        get
        {
            return _scrollRectComponent.velocity;
        }
    }
    /// <summary>
    /// Gets the initialization status of the <c>FAScrollSnapBehaviour</c>
    /// </summary>
    public bool DidInit { get; private set; }
    /// <summary>
    /// Gets the scrolling orientation of the <c>FAScrollSnapBehaviour</c>
    /// </summary>
    public bool IsHorizontal { get { return _horizontal; } }
    #endregion
    #region Public Fields

    /// <summary>
    /// List of all <see cref="FAScrollSnapItem"/> child items.
    /// </summary>
    public List<FAScrollSnapItem> Items { get { return items; } }
    /// <summary>
    /// Gets or sets starting page index - starting from 0. Default is 0. Should be set before initialization
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("Set starting page index - starting from 0")]
    public int startingPage = 0;

    /// <summary>
    /// Gets or sets the threshold time for fast swipe in seconds. Default is 0.3f.
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("Threshold time for fast swipe in seconds")]
    public float fastSwipeThresholdTime = 0.3f;

    /// <summary>
    /// Gets or sets threshold time for fast swipe in (unscaled) pixels. Default is 50.
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
    public int fastSwipeThresholdDistance = 50;

    [Space(20)]
    /// <summary>
    /// Gets or sets how fast will a page lerp to target position. Default is 5.
    /// </summary>
    [Tooltip("How fast will page lerp to target position")]
    public float decelerationRate = 5f;

    /// <summary>
    /// If true, will snap to the first item, if false, will decelarate freely then snap when decelarationLimit is reached.
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("If true, will snap to the first item, if false, will decelarate freely then snap when decelarationLimit is reached.")]
    public bool snapToFirst = false;

    /// <summary>
    /// When decelarating freely, Gets or sets how slowed should the scrolling speed be before snapping.
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("When decelarating freely, how slowed should the scrolling speed be before snapping.")]
    public float decelarationLimit = 10f;

    [Space(20)]
    /// <summary>
    /// If true, the items of the Scroll View will be centered in the viewport.
    /// </summary>
    [Tooltip("If true, the items of the Scroll View will be centered in the viewport.")]
    public bool centerSelectedItem = true;
    /// <summary>
    /// Gets or sets if the this forces to resize the item according to one of the multipliers.
    /// See <see cref="widthMultiplier"/> and <see cref="heightMultiplier"/>/>
    /// </summary>
    /// <remarks></remarks>
    [Tooltip("Forces the script to resize the item according to one of the multipliers below.")]
    public bool forceSize = false;

    /// <summary>
    /// Gets or sets how wide should one item be in relation to the ScrollRect Width.
    /// <para><strong>1f</strong> means same width, <strong>0.5f</strong> means half width.</para>
    /// <para>Works only if ScrollRect is horizontal.</para>
    /// </summary>
    [Tooltip("How wide should one item be in relation to the ScrollRect Width. 1f means same width, 0.5f means half width. Works only if ScrollRect is horizontal.")]
    public float widthMultiplier = 0.6f;

    /// <summary>
    /// Gets or sets how tall should one item be in relation to the ScrollRect Height.
    /// <para><strong>1f</strong> means same height, <strong>0.5f</strong> means half height.</para>
    /// <para>Works only if ScrollRect is vertical.</para>
    /// </summary>
    [Tooltip("How tall should one item be in relation to the ScrollRect Height. 1f means same height, 0.5f means half height. Works only if ScrollRect is vertical.")]
    public float heightMultiplier = 0.6f;


    [Space(20)]
    /// <summary>
    /// Button to go to the previous page (optional)
    /// </summary>
    [Tooltip("Button to go to the previous page (optional).")]
    public GameObject prevButton;

    /// <summary>
    /// Button to go to the next page (optional)
    /// </summary>
    [Tooltip("Button to go to the next page (optional).")]
    public GameObject nextButton;

    /// <summary>
    /// If you have inner scroll views that scroll in the opposite direction of this ScrollSnapRect, you should add them here so the Drag Event is propagaded to this ScrollSnapRect.
    /// Should be set in inspector, otherwise please initialize the List.
    /// </summary>
    [Tooltip("If you have inner scroll views that scroll in the opposite direction of this ScrollSnapRect, you should add them here so the Drag Event is propagaded to this ScrollSnapRect (optional).")]
    public List<FAScrollRect> innerScrolls;
    #endregion
    #region Event Classes
    /// <summary>
    /// Occurs when the <c>ScrollRect</c> is scrolled. 
    /// <para>The passed <typeparamref name="Vector2"/> argument holds the <c>ScrollRect</c>'s content current anchoredPosition</para>
    /// <para>Useful for doing animations on a single item and compare positions.</para>
    /// </summary>
    /// <typeparam name="Vector2">The current anchored position of the <c>ScrollRect</c>'s content</typeparam>
    public class ScrollEvent : UnityEvent<Vector2> { }

    /// <summary>
    /// Occurs when the <c>ScrollRect</c> is scrolled.
    /// <para>The passed <typeparamref name="float"/> argument holds the normalized value of the scrolled percentage.</para>
    /// <para><strong>0f</strong> means first page is selected. <strong>1f</strong> means last page is selected.</para>
    /// <para>Useful for doing animations on a group of items.</para>
    /// </summary>
    /// <typeparam name="float">The current scrolled percentage of the <c>ScrollRect</c>'s content. 
    /// </typeparam> 
    public class ScrollPercentageEvent : UnityEvent<float> { }
    /// <summary>
    /// Occurs when the ScrollView locks on a page and scrolls to it.
    /// <para>Useful for doing animations after a page was found and selected.</para>
    /// <para>
    /// If buttons are used for navigation, this event occurs on Prev/Next button tap.
    /// </para>
    /// <para>
    /// If scrolling with <see cref="snapToFirst"/> set to <strong>true</strong>, this occurs as soon as the dragging end.
    /// </para>
    /// <para>
    /// If scrolling with <see cref="snapToFirst"/> set to <strong>false</strong>, this occurs after the scroll speed is slowed enough so it locks to a page.
    /// </para>
    /// </summary>
    public class ScrollToPageEvent : UnityEvent<int> { }
    /// <summary>
    /// Occurs when scrolling has started.
    /// </summary>
    public class ScrollStartedEvent : UnityEvent { }

    /// <summary>
    /// Occurs when scrolling has completely ended.
    /// <para>Useful for cleaning up effects and animations.</para>
    /// </summary>
    public class ScrollEndedEvent : UnityEvent<int> { }

    public class OnMouseDownEvent : UnityEvent { }
    public class OnMouseUpEvent : UnityEvent { }

    #endregion
    #region Events

    /// <summary>
    /// Occurs when the <c>ScrollRect</c> is scrolled. 
    /// <para>The passed <typeparamref name="Vector2"/> argument holds the <c>ScrollRect</c>'s content current anchoredPosition</para>
    /// <para>Useful for doing animations on a single item and compare positions.</para>
    /// </summary>
    /// <typeparam name="Vector2">The current anchored position of the <c>ScrollRect</c>'s content</typeparam>
    public ScrollEvent onScrolled;
    /// <summary>
    /// Occurs when the <c>ScrollRect</c> is scrolled.
    /// <para>The passed <typeparamref name="float"/> argument holds the normalized value of the scrolled percentage.</para>
    /// <para><strong>0f</strong> means first page is selected. <strong>1f</strong> means last page is selected.</para>
    /// <para>Useful for doing animations on a group of items.</para>
    /// </summary>
    /// <typeparam name="float">The current scrolled percentage of the <c>ScrollRect</c>'s content. 
    /// </typeparam> 
    public ScrollPercentageEvent onScrolledPercentage;

    /// <summary>
    /// Occurs when the ScrollView locks on a page and scrolls to it.
    /// <para>Useful for doing animations after a page was found and selected.</para>
    /// <para>
    /// If buttons are used for navigation, this event occurs on Prev/Next button tap.
    /// </para>
    /// <para>
    /// If scrolling with <see cref="snapToFirst"/> set to <strong>true</strong>, this occurs as soon as the dragging end.
    /// </para>
    /// <para>
    /// If scrolling with <see cref="snapToFirst"/> set to <strong>false</strong>, this occurs after the scroll speed is slowed enough so it locks to a page.
    /// </para>
    /// </summary>
    public ScrollToPageEvent onScrollToPage;
    /// <summary>
    /// Occurs when scrolling has started.
    /// </summary>
    public ScrollStartedEvent onScrollStarted;
    
    /// <summary>
    /// Occurs when scrolling has completely ended.
    /// <para>Useful for cleaning up effects and animations.</para>
    /// </summary>
    public ScrollEndedEvent onScrollEnded;

    public OnMouseDownEvent onMouseDown;
    public OnMouseUpEvent onMouseUp;

    #endregion
    #region Private Fields
    // fast swipes should be fast and short. If too long, then it is not fast swipe
    private int _fastSwipeThresholdMaxLimit;

    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;
    private RectTransform _container;

    private bool _horizontal;

    // number of pages in container
    private int _pageCount;
    private int _currentPage;

    // whether lerping is in progress and target lerp position
    private bool _lerp, _didFindNearest, _isDecelarating;
    private Vector2 _lerpTo;

    // target position of every page
    private List<Vector2> _pagePositions = new List<Vector2>();
    private List<FAScrollSnapItem> items = new List<FAScrollSnapItem>();

    // in draggging, when dragging started and where it started
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    int padding;
    bool startedTouching = false;

    #endregion
    #region Public Methods
    /// <summary>
    /// Setup can be called at any time to setup the behaviour. 
    /// <para>Should be called on Start or when the number of items in the Scroll View changes.</para>
    /// </summary>
    public void Setup()
    {
        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();

        // is it horizontal or vertical scrollrect
        if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
        {
            _horizontal = true;
        }
        else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
        {
            _horizontal = false;
        }
        else
        {
            Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
            _horizontal = true;
        }
        // setup inner scrolls
        foreach (FAScrollRect item in innerScrolls)
        {
            item.mOutterScroll = _scrollRectComponent;
            item.mOutterScrollRect = this;
        }

        _container = _scrollRectComponent.content;

        _pageCount = _container.childCount;



        _lerp = false;
        // init
        if(gameObject.activeInHierarchy)
            StartCoroutine(SetPagePositions());

    }
    /// <summary>
    /// Smoothly lerps the Scroll View to the page at the given index.
    /// <para>Can be called on button click.</para>
    /// </summary>
    /// <param name="aPageIndex">Zero based index of the page,</param>
    public void LerpToPage(int aPageIndex)
    {
        if (!DidInit)
            return;
        if (_pagePositions == null)
            return;
        if (_pagePositions.Count <= 0)
            return;
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _lerpTo = _pagePositions[aPageIndex];
        _lerp = true;
        _currentPage = aPageIndex;
    }
    //public void JumpToPage(int aPageIndex)
    //{
    //    aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
    //    _lerpTo = _pagePositions[aPageIndex];
    //    _container.anchoredPosition = _lerpTo;
    //    _currentPage = aPageIndex;
    //    onScrollToPage?.Invoke(aPageIndex);
    //}
    public void SetScrollPosition(float pos)
    {
        _container.anchoredPosition = new Vector2(IsHorizontal ? pos : _container.anchoredPosition.x, IsHorizontal ? _container.anchoredPosition.y : pos);
    }

    #endregion
    #region MonoBehaviour Callbacks
    private void Awake()
    {

        if (onMouseDown == null)
            onMouseDown = new OnMouseDownEvent();
        if (onMouseUp == null)
            onMouseUp = new OnMouseUpEvent();
        if (onScrolled == null)
            onScrolled = new ScrollEvent();
        if (onScrolledPercentage == null)
            onScrolledPercentage = new ScrollPercentageEvent();
        if (onScrollToPage == null)
            onScrollToPage = new ScrollToPageEvent();
        if (onScrollStarted == null)
            onScrollStarted = new ScrollStartedEvent();
        if (onScrollEnded == null)
            onScrollEnded = new ScrollEndedEvent();

    }
    //------------------------------------------------------------------------
    void Start()
    {
        Setup();

        // prev and next buttons
        if (nextButton)
            nextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

        if (prevButton)
            prevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
    }



    //------------------------------------------------------------------------
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !startedTouching)
        {
            startedTouching = true;
            onMouseDown?.Invoke();
        }
        else if(Input.GetMouseButtonUp(0) && startedTouching)
        {
            startedTouching = false;
            onMouseUp?.Invoke();

        }
        // if moving to target position
        if (_lerp)
        {
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);

            InvokeScrollEvents();
            // time to stop lerping?
            if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 10f)
            {
                // clear any scrollrect move that may interfere with our lerping
                _scrollRectComponent.velocity = Vector2.zero;
                // snap to target and stop lerping
                _container.anchoredPosition = _lerpTo;

                _lerp = false;
                onScrollEnded?.Invoke(_currentPage);
            }

        }
        if (_isDecelarating)
        {
            // prevent overshooting with values greater than 1
            float velocity = _horizontal ? _scrollRectComponent.velocity.x : _scrollRectComponent.velocity.y;

            InvokeScrollEvents();
            if (Mathf.Abs(velocity) < decelarationLimit)
            {
                if (!_didFindNearest)
                {
                    _isDecelarating = false;
                    _didFindNearest = true;
                    int nearestPage = GetNearestPage();
                    onScrollToPage?.Invoke(nearestPage);
                    LerpToPage(nearestPage);
                }
            }

        }
    }
    private void OnDisable()
    {
        onScrolled?.RemoveAllListeners();
    }
    #endregion
    #region Private Methods
    //------------------------------------------------------------------------
    private IEnumerator SetPagePositions()
    {
        //LayoutGroups need one frame to set the size of its elements
        yield return null;
        if (_container.childCount <= 0)
            yield break;
        float width = 0;
        float height = 0;
        //Set size and padding according to rules
        if (_horizontal)
        {

            float rectWidth = _scrollRectRect.rect.width;
            HorizontalLayoutGroup hlg = _container.GetComponent<HorizontalLayoutGroup>();
            if (hlg == null)
                hlg = _container.gameObject.AddComponent<HorizontalLayoutGroup>();

            if (centerSelectedItem)
            {
                if (forceSize)
                {
                    width = _scrollRectRect.rect.width * widthMultiplier;
                }
                else
                {
                    width = _container.GetChild(0).GetComponent<LayoutElement>().preferredWidth;
                }
                int padding = (int)(rectWidth - width) / 2;
                hlg.padding = new RectOffset(padding, padding, 0, 0);
            }

            padding = hlg.padding.left;
        }
        else
        {
            float rectHeight = _scrollRectRect.rect.height;
            VerticalLayoutGroup vlg = _container.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
                vlg = _container.gameObject.AddComponent<VerticalLayoutGroup>();

            if (centerSelectedItem)
            {
                if (forceSize)
                {
                    height = _scrollRectRect.rect.height * heightMultiplier;
                }
                else
                {
                    height = _container.GetChild(0).GetComponent<LayoutElement>().preferredHeight;
                }
                int padding = (int)(rectHeight - height) / 2;
                vlg.padding = new RectOffset(0, 0, padding, padding);
            }

            padding = vlg.padding.top;
        }


        _fastSwipeThresholdMaxLimit = _horizontal ? (int)_scrollRectRect.rect.width : (int)_scrollRectRect.rect.height;

        // delete any previous settings
        _pagePositions.Clear();
        items.Clear();
        // iterate through all container childern and set their positions

        yield return null;
        for (int i = 0; i < _pageCount; i++)
        {
            RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
            if (forceSize)
            {

                LayoutElement layoutElement = child.GetComponent<LayoutElement>();
                if (_horizontal)
                {
                    layoutElement.preferredWidth = width;
                    child.sizeDelta = new Vector2(width, child.rect.height);
                }
                else
                {
                    layoutElement.preferredHeight = height;
                    child.sizeDelta = new Vector2(child.rect.width, height);

                }

                yield return null;
            }

            FAScrollSnapItem item = child.GetComponent<FAScrollSnapItem>();
            if (item == null)
            {
                item = child.gameObject.AddComponent<FAScrollSnapItem>();
            }

            items.Add(item);
            if (_horizontal)
                _pagePositions.Add(new Vector2(-child.anchoredPosition.x + (child.sizeDelta.x * child.pivot.x) + padding, 0));
            else
                _pagePositions.Add(new Vector2(0, -child.anchoredPosition.y - (child.sizeDelta.y * child.pivot.y) - padding));

        }
        float difference = 0;
        //Find difference between two pages
        if (_pagePositions.Count > 1)
        {
            float a = _horizontal ? Mathf.Abs(_pagePositions[1].x) : Mathf.Abs(_pagePositions[1].y);
            float b = _horizontal ? Mathf.Abs(_pagePositions[0].x) : Mathf.Abs(_pagePositions[0].y);
            difference = Mathf.Abs(a - b);
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].hasAnimations)
            {
                FAScrollSnapItemSettings settings = new FAScrollSnapItemSettings()
                {
                    Position = _horizontal ? _pagePositions[i].x : _pagePositions[i].y,
                    Difference = difference,
                    IsHorisontal = _horizontal
                };
                items[i].Setup(settings);
                onScrolled.AddListener(items[i].OnScrollHandler);
            }
        }
        if (items.Count > 0)
        {

            SetPage(startingPage);
            items[startingPage].SetSelected();

            DidInit = true;
        }

        //LerpToPage(0);
    }

    //------------------------------------------------------------------------
    private void SetPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        if (_pagePositions == null)
            return;
        if (_pagePositions.Count <= 0)
            return;
        _container.anchoredPosition = _pagePositions[aPageIndex];
        InvokeScrollEvents();
        _currentPage = aPageIndex;
    }

    public void InvokeScrollEvents()
    {
        onScrolled?.Invoke(_container.anchoredPosition);

        if (onScrolledPercentage == null)
            return;

        //some math to find scrolledPercentage
        float t;
        if (_pagePositions.Count <= 1)
        {
            t = 1;
        }
        else if (_horizontal)
        {
            t = Mathf.InverseLerp(_pagePositions[0].x, _pagePositions[_pagePositions.Count - 1].x, _container.anchoredPosition.x);
        }
        else
        {
            t = Mathf.InverseLerp(_pagePositions[0].y, _pagePositions[_pagePositions.Count - 1].y, _container.anchoredPosition.y);
        }
        onScrolledPercentage?.Invoke(t);
    }


    //------------------------------------------------------------------------
    private void NextScreen()
    {
        int p = _currentPage + 1;
        if (p >= _pageCount)
            return;
        LerpToPage(p);
        onScrollStarted?.Invoke();
        onScrollToPage?.Invoke(p);
    }

    //------------------------------------------------------------------------
    private void PreviousScreen()
    {
        int p = _currentPage - 1;
        if (p < 0)
            return;
        LerpToPage(p);
        onScrollStarted?.Invoke();
        onScrollToPage?.Invoke(p);
    }
    //------------------------------------------------------------------------
    private int GetNearestPage()
    {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = _container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }
    #endregion
    #region DragHandlers
    //------------------------------------------------------------------------
    public void OnBeginDrag(PointerEventData aEventData)
    {
        // if currently lerping, then stop it as user is draging
        _lerp = false;
        // not dragging yet
        _didFindNearest = false;
        _dragging = false;
        onScrollStarted?.Invoke();
    }

    //------------------------------------------------------------------------
    public void OnEndDrag(PointerEventData aEventData)
    {
        startedTouching = false;
        // how much was container's content dragged
        float difference;
        if (_horizontal)
        {
            difference = _startPosition.x - _container.anchoredPosition.x;
        }
        else
        {
            difference = -(_startPosition.y - _container.anchoredPosition.y);
        }

        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit)
        {
            if (difference > 0)
            {
                NextScreen();
            }
            else
            {
                PreviousScreen();
            }
        }
        else
        {
            if (snapToFirst)
            {
                // snap to first page
                int nearestPage = GetNearestPage();
                onScrollToPage?.Invoke(nearestPage);

                LerpToPage(nearestPage);
            }
            else
            {
                //naturally decelarate
                _isDecelarating = true;

            }
        }

        _dragging = false;
    }
    
    //------------------------------------------------------------------------
    public void OnDrag(PointerEventData aEventData)
    {
        if (!_dragging)
        {
            // dragging started
            _dragging = true;
            // save time - unscaled so pausing with Time.scale should not affect it
            _timeStamp = Time.unscaledTime;
            // save current position of cointainer
            _startPosition = _container.anchoredPosition;
        }
        else
        {
            InvokeScrollEvents();
        }
    }
    #endregion
}


