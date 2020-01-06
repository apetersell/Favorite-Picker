using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public string listTitle;
    public List<string> titles = new List<string>();
    public struct Element
    {
        public string title;
        public List<Element> beatenBy;
    }
    private List<Element> pool = new List<Element>();
    public List<string> winners = new List<string>();
    private List<Element> eliminated = new List<Element>();
    public List<int> selections = new List<int>();
    private List<Element> onDisplay = new List<Element>();
    public ElementButton[] buttons;
    private int poolSize;
    private int displaySize;
    public WinnerFeed feed;
    private int elementsRanked;
    private bool done;

    //Stats Stuff
    public Text totalElements;
    public Text ranked;
    public Text unrankedElements;
    public Text elementsInPool;
    public Text elementsEliminated;
    public Text mostRecent;

    //List Stuff
    public Text listTitleObject;
    public Transform listContainer;
    public GameObject listElementPrefab;

    public GameObject finishedText;
    public GameObject choiceGroup;

    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
        SetUpNewRound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializePool()
    {
        for (int i = 0; i < titles.Count; i++)
        {
            Element newElement = new Element();
            newElement.title = titles[i];
            newElement.beatenBy = new List<Element>();
            pool.Add(newElement);
        }
        poolSize = pool.Count;
        elementsRanked = 0;
        for (int i = 0; i < feed.slots.Length; i++)
        {
            var text = feed.slots[i];
            text.text = "";
        }
        done = false;
        UpdateStats();
        mostRecent.text = "";
        choiceGroup.SetActive(true);
        finishedText.SetActive(false);
        listTitleObject.text = listTitle;
    }

    void SetUpNewRound()
    {
        onDisplay.Clear();
        selections.Clear();
        UpdateStats();
        displaySize = CalculateDispalySize();
        for (int i = 0; i < displaySize; i++)
        {
            Element element = chooseRandomFromPool();
            pool.Remove(element);
            onDisplay.Add(element);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            bool shouldActivate = i < displaySize;
            button.gameObject.SetActive(shouldActivate);
            if (shouldActivate)
            {
                var element = onDisplay[i];
                button.index = i;
                button.BG.color = Color.white;
                button.textComponent.text = element.title;
            }
        }

    }

    public void ConfirmSelection()
    {
        if (selections.Count > 0 && !done)
        {
            List<Element> roundWinners = new List<Element>();
            List<Element> roundLosers = new List<Element>();
            for (int i = 0; i < onDisplay.Count; i++)
            {
                var element = onDisplay[i];
                if (selections.Contains(i))
                {
                    roundWinners.Add(element);
                }
                else
                {
                    roundLosers.Add(element);
                }
            }
            if (roundLosers.Count > 0)
            {
                for (int i = 0; i < roundLosers.Count; i++)
                {
                    var losingElement = roundLosers[i];
                    foreach (var win in roundWinners)
                    {
                        losingElement.beatenBy.Add(win);
                    }
                    eliminated.Add(losingElement);
                }
            }
            for (int i = 0; i < roundWinners.Count; i++)
            {
                var winningElement = roundWinners[i];
                pool.Add(winningElement);
            }
            if (pool.Count == 1)
            {
                AddWinner();
            }
            else
            {
                SetUpNewRound();
            }
        }

       
    }

    void AddWinner()
    {
        Element winner = pool[0];
        winners.Add(winner.title);
        elementsRanked++;
        string displayString = string.Format("#{0}: {1}", elementsRanked, winner.title);
        mostRecent.text = displayString;
        AddToList(displayString);
        feed.UpdateFeed(displayString);
        pool.Clear();
        List<Element> reviveList = new List<Element>();
        for (int i = 0; i < eliminated.Count; i++)
        {
            Element element = eliminated[i];
            if (element.beatenBy.Contains(winner))
            {
                element.beatenBy.Remove(winner);
                if (element.beatenBy.Count == 0)
                {
                    reviveList.Add(element);
                }
            }
        }
        if (reviveList.Count > 0)
        {
            for (int i = 0; i < reviveList.Count; i++)
            {
                Element element = reviveList[i];
                eliminated.Remove(element);
                pool.Add(element);
            }
        }
        if (pool.Count > 1)
        {
            SetUpNewRound();
        }
        else if (pool.Count == 1)
        {
            AddWinner();
        }
        else
        {
            UpdateStats();
            done = true;
            choiceGroup.SetActive(false);
            finishedText.SetActive(true);
            feed.UpdateFeed("All Elements Ranked");
        }
    }

    int CalculateDispalySize()
    {
        float percent = (float)pool.Count / (float)poolSize;
        float lerpFloat = Mathf.Lerp(2f, (float)buttons.Length, percent);
        int result = Mathf.RoundToInt(lerpFloat);
        if (result > pool.Count)
        {
            result = pool.Count;
        }
        return result;
    }

    Element chooseRandomFromPool()
    {
        Element chosen;
        int rando = Random.Range(0, pool.Count);
        chosen = pool[rando];
        return chosen;
    }

    private void UpdateStats()
    {
        int inRotation = pool.Count;
        int alreadyRanked = poolSize - elementsRanked;
        string totalString = string.Format("Total Elements: {0}", poolSize);
        string rankedString = string.Format("Ranked: {0}", elementsRanked);
        string unrankedString = string.Format("Remaining: {0}", alreadyRanked);
        string elementsInPoolString = string.Format("In Pool: {0}", + inRotation);
        string eliminString = string.Format("Eliminated: {0}", eliminated.Count);
        totalElements.text = totalString;
        ranked.text = rankedString;
        unrankedElements.text = unrankedString;
        elementsInPool.text = elementsInPoolString;
        elementsEliminated.text = eliminString;
    }

    private void AddToList(string sent)
    {
        GameObject newListItem = Instantiate(listElementPrefab) as GameObject;
        newListItem.transform.SetParent(listContainer);
        newListItem.transform.localScale = Vector3.one;
        Text text = newListItem.GetComponent<Text>();
        text.color = Color.green;
        text.text = sent;
    }
}
