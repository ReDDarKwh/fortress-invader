using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Scripts.Spells;
using System;
using Scripts.Missions;

public class PlayerHUDController : MonoBehaviour
{

    // private class SpellSlot
    // {
    //     public Spell spell;
    //     public Selectable slot;

    //     public selected
    // }

    public Image playerHealthBar;
    public Image playerManaBar;
    public PlayerCharacter playerCharacter;

    public GameObject spellsBar;

    public GameObject spellSlotPrefab;

    public GameObject missionTargetPrefab;

    public GameObject minimapItemsContainer;

    public GameObject spellMouseUI;

    public Text castSpellText;

    public GameObject spellSelectionImage;

    public Text scoreText;

    public Text timerText;

    public GameObject currentMissionElement;

    public Text currentMissionName;
    public Text currentMissionDescription;

    public RectTransform minimapElement;
    public RectTransform minimapRawImage;
    public Camera minimapCam;

    public float minimapRadius;

    private float vel = 0.0f;

    private FortressSceneManager sceneManager;
    private MissionsController missionController;


    //rivate List<SpellSlot>

    // Use this for initialization
    void Start()
    {

        sceneManager = GameObject.FindGameObjectWithTag("SceneManager")
        .GetComponent<FortressSceneManager>();

        missionController = SharedSceneController.Instance.missionController;

        playerCharacter.spellCaster.currentMana.Subscribe(x =>
        {
            playerManaBar.fillAmount = x / playerCharacter.spellCaster.maxMana.Value;
        });

        playerCharacter.spellCaster.selectedSpell.Subscribe(x =>
        {

            castSpellText.text = x?.spellName ?? "";
            switch (x?.spellTarget)
            {
                case SpellTarget.MISSILE:
                    spellMouseUI.SetActive(true);
                    break;

                default:
                    spellMouseUI.SetActive(false);
                    break;
            }
        });

        sceneManager.score.Subscribe(x =>
        {
            scoreText.text = x.ToString();
        });


        playerCharacter.showCurrentMissionElement.Subscribe(x =>
        {
            currentMissionElement.SetActive(x);
        });

        SharedSceneController.Instance.missionController.selectedMission.Subscribe(x =>
        {
            currentMissionName.text = x?.missionName ?? "No mission selected";
            currentMissionDescription.text = x?.desc ?? $"Open the fortress menu to select your next mission.";
        });

        playerCharacter.showMinimap.Subscribe(x =>
        {
            minimapElement.gameObject.SetActive(x);
        });

        for (var i = 1; i <= playerCharacter.spellCaster.spells.Count; i++)
        {
            createSpellSlot(i, playerCharacter.spellCaster.spells[i - 1]);
        }
        // create "no spell" slot
        createSpellSlot(0, null);



    }


    private void createSpellSlot(int number, Spell spell)
    {
        var slot = Instantiate(spellSlotPrefab, Vector3.zero, Quaternion.identity);
        var slotController = slot.GetComponent<SpellSlotController>();

        slotController.spell = spell;
        slotController.number = number;
        slotController.spellCaster = playerCharacter.spellCaster;
        slot.transform.SetParent(spellsBar.transform);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMinimapItems();

        playerHealthBar.fillAmount = Mathf.SmoothDamp(
            playerHealthBar.fillAmount,
            playerCharacter.character.currentHealth / playerCharacter.character.maxHealth,
            ref vel, 0.1f
        );

        // missile number selected display logic
        if (playerCharacter.spellCaster.selectedSpell.Value != null && playerCharacter.spellCaster.selectedSpell.Value.spellTarget == SpellTarget.MISSILE)
        {
            var missileTarget = playerCharacter.spellCaster.spellTarget.GetComponent<MagicTargetMissileController>();
            var missileTargetSelectedText = spellMouseUI.GetComponent<Text>();

            missileTargetSelectedText.text = $"{missileTarget.selectedCharacters.Count }/{missileTarget.maxTargets}";
            spellMouseUI.transform.position = Input.mousePosition;
        }

        // missile selection box logic
        if (playerCharacter.selectingTargets)
        {
            spellSelectionImage.SetActive(true);

            var rectTransform = spellSelectionImage.GetComponent<RectTransform>();


            var selectingViewportPos = Camera.main.WorldToScreenPoint(playerCharacter.selectingTargetsStartPos);

            var width = Input.mousePosition.x - selectingViewportPos.x;
            var height = selectingViewportPos.y - Input.mousePosition.y;

            rectTransform.position = selectingViewportPos;

            rectTransform.position = new Vector2(width > 0 ? selectingViewportPos.x : Input.mousePosition.x,

             height > 0 ? selectingViewportPos.y : Input.mousePosition.y);

            rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        }
        else
        {
            spellSelectionImage.SetActive(false);
        }


        // timer display
        var remainingTime = sceneManager.timeBeforeGameover - (Time.unscaledTime - sceneManager.timeSinceLevelLoad);
        var time = new TimeSpan(0, 0, Convert.ToInt32(remainingTime));

        timerText.text = time.ToString(@"mm\:ss");

    }

    private void UpdateMinimapItems()
    {
        var positions = missionController.GetCurrentObjectivePositions();

        var diffMissionTargetsAndPositions =
        positions.Count - minimapItemsContainer.transform.childCount;

        for (var i = 0; i < Mathf.Abs(diffMissionTargetsAndPositions); i++)
        {
            if (diffMissionTargetsAndPositions > 0)
            {
                Instantiate(missionTargetPrefab, minimapItemsContainer.transform, false);
            }
            else
            {
                Destroy(minimapItemsContainer.transform.GetChild(i).gameObject);
            }
        }

        // In theory the number of objective positions should match the number of MissionTarget UI elements by this point
        for (var i = 0; i < positions.Count; i++)
        {
            var playerToMissionObjective =
            minimapCam.WorldToScreenPoint(positions[i]) - new Vector3(minimapRawImage.rect.width / 2,
            minimapRawImage.rect.height / 2);

            minimapItemsContainer.transform.GetChild(i).transform.localPosition =
            playerToMissionObjective.normalized * (Mathf.Min(playerToMissionObjective.magnitude, minimapRadius));
        }
    }
}
