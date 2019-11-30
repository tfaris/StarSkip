using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public Game game;

    public Image select1;
    public Image select2;
    public Image select3;
    public Image select4;
    public Image select5;
    public Image select6;
    public Image select7;
    public Image select8;
    public Image select9;

    public Image star1;
    public Image star2;
    public Image star3;
    public Image star4;
    public Image star5;
    public Image star6;
    public Image star7;
    public Image star8;
    public Image star9;

    public Image warpJuice1;
    public Image warpJuice2;
    public Image warpJuice3;
    public Image warpJuice4;
    public Image warpJuice5;
    public Image warpJuice6;
    public Image warpJuice7;
    public Image warpJuice8;
    public Image warpJuice9;
    public Image warpJuice10;

    public Image hpBar;

    public Text textForPlayer;

    public Text totalExplored;

    public UItext textManager;
    public float textTimer;

    public AudioClip notificationSound;


    List<Image> juices = new List<Image>();
    List<Image> stars = new List<Image>();
    List<Image> selections = new List<Image>();
    public List<Image> upgrades = new List<Image>();
    List<Image> curUpgrdes = new List<Image>();

    float _lastSelectionX, _textTimerCounter;
    bool _showingText;
    Queue<string> _messageQueue = new Queue<string>();

    public int WarpCount
    {
        get
        {
            if (Game.Instance.playerShip)
            {
                return Game.Instance.playerShip.warpPoints;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (Game.Instance.playerShip)
            {
                Game.Instance.playerShip.warpPoints = value;
            }
        }
    }

    public int CurrentUpgradeSelection
    {
        get
        {
            if (Game.Instance.playerShip)
            {
                return (int)Game.Instance.playerShip.CurrentUpgradeType;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (Game.Instance.playerShip)
            {
                Game.Instance.playerShip.CurrentUpgradeType = (PlayerShip.UpgradeType)value;
            }
        }
    }

    void Start()
    {
        juices.Add(warpJuice1);
        juices.Add(warpJuice2);
        juices.Add(warpJuice3);
        juices.Add(warpJuice4);
        juices.Add(warpJuice5);
        juices.Add(warpJuice6);
        juices.Add(warpJuice7);
        juices.Add(warpJuice8);
        juices.Add(warpJuice9);
        juices.Add(warpJuice10);

        stars.Add(star1);
        stars.Add(star2);
        stars.Add(star3);
        stars.Add(star4);
        stars.Add(star5);
        stars.Add(star6);
        stars.Add(star7);
        stars.Add(star8);
        stars.Add(star9);

        // standard
        selections.Add(select1);
        // scatter
        selections.Add(select2);
        // pierce
        selections.Add(select3);
        // missile
        selections.Add(select4);
        // mine
        selections.Add(select5);
        // super laser
        selections.Add(select6);
        // rear shot
        selections.Add(select7);
        // HEALTH NOT ARMOR
        selections.Add(select8);
        // speed
        selections.Add(select9);
    }

    void Update()
    {
        totalExplored.text = game.ExplorationCount.ToString();

        var player = Game.Instance.playerShip;
        if (player)
        {
            hpBar.fillAmount = player.health / (float)player.maxHealth;
        }
        else
        {
            hpBar.fillAmount = 0;
        }

        // text
        if (!_showingText && _messageQueue.Count > 0)
        {
            // show the next message
            textForPlayer.text = _messageQueue.Dequeue();
            textForPlayer.enabled = true;
            textForPlayer.transform.parent.gameObject.SetActive(true);
        
            if (notificationSound)
            {
                Game.Instance.effectsAudioSource.PlayOneShot(notificationSound);
            }
            
            _textTimerCounter = 0;
            _showingText = true;
        }
        else if (_showingText)
        {
            if (_textTimerCounter >= textTimer)
            {
                textForPlayer.enabled = false;
                textForPlayer.transform.parent.gameObject.SetActive(false);
                textForPlayer.text = string.Empty;
                _showingText = false;
                _textTimerCounter = 0;
            }
            _textTimerCounter += Time.deltaTime;
        }
        //

        for (int i=0; i < juices.Count; i++)
        {
            juices[i].enabled = (i+1) <= WarpCount;
        }

        if (CurrentUpgradeSelection != (int)PlayerShip.UpgradeType.NoUpgrades)
        {
            // if (Input.GetButtonDown("Upgrade Selection Down"))
            // {
            //     BumpUpgradeSelection(-1);
            // }

            // if (Input.GetButtonDown("Upgrade Selection Up"))
            // {
            //     BumpUpgradeSelection(1);
            // }
            
            float axVal = Input.GetAxis("Upgrade Selection");
            if (axVal != _lastSelectionX)
            {
                if (axVal < 0)
                {
                    BumpUpgradeSelection(-1);
                }
                else if (axVal > 0)
                {
                    BumpUpgradeSelection(1);
                }
                _lastSelectionX = axVal;
            }

            UpdateStarsAndSelections();

            // Automatically bump the upgrade if current is not upgradeable.
            IUpgradeable upgradeable = Game.Instance.playerShip.GetUpgradeable();
            if (!upgradeable.CanUpgrade())
            {
                BumpUpgradeSelection(1);
            }
        }
        else
        {
            // Try to find a new thing we can upgrade.
            BumpUpgradeSelection(1);
            UpdateStarsAndSelections();
        }
    }

    void UpdateStarsAndSelections()
    {
        for (int i=0; i < selections.Count; i++)
        {
            selections[i].GetComponent<Image>().enabled =
                i == (int)Game.Instance.playerShip.CurrentUpgradeType;
                
            PlayerShip.UpgradeType ut = (PlayerShip.UpgradeType) i;
            IUpgradeable ug = Game.Instance.playerShip.GetUpgradeable(ut);
            
            stars[i].enabled = ug.HasMaxUpgrade();
            upgrades[i].enabled = !(ug is PlayerShip.NonUpgradeable);
        }
    }

    void BumpUpgradeSelection(int adjust)
    {
        int newIndex = CurrentUpgradeSelection + adjust;
        int count = 0;
        for (int i = CurrentUpgradeSelection + adjust; ;i += adjust)
        {
            if (i < 0)
            {
                i = selections.Count - 1;
            }
            else if (i >= selections.Count)
            {
                i = 0;
            }
            IUpgradeable ug = Game.Instance.playerShip.GetUpgradeable((PlayerShip.UpgradeType)i);
            if (ug.CanUpgrade())
            {
                CurrentUpgradeSelection = i;
                break;
            }
            count++;

            if (count >= selections.Count)
            {
                CurrentUpgradeSelection = (int)PlayerShip.UpgradeType.NoUpgrades;
                break;
            }
        }
    }

    public void ShowMessage(UItext.MessageType messageType, params string[] formatArgs)
    {
        string msg = this.textManager.GetMessage(messageType, formatArgs);
        _messageQueue.Enqueue(msg);
    }
}
