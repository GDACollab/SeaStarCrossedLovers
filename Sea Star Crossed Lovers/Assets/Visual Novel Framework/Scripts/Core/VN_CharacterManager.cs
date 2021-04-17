﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VN_CharacterManager : MonoBehaviour
{
	[Tooltip("CharacterData of the player in the VN")]
	[SerializeField] private CharacterData PlayerCharacterData;
	[Tooltip("Generic VN_Character GameObjects; There should be only 2 in a scene")]
	public List<VN_Character> CharacterObjects;
	[Tooltip("List of needed character data to pull from")]
	public List<CharacterData> AllCharacterData;

	public Color speakingColor;
	public Color nonSpeakingColor;
	public float speakerLightDuration;
	public Ease speakerLightEase;

	public Vector2 speakingOffset;

	private VN_Manager manager;

    public void Construct(VN_Manager manager)
    {
        this.manager = manager;
	}

	public IEnumerator UpdateSpeakerLight(VN_Character speaker)
    {
		foreach (VN_Character character in CharacterObjects)
        {
			if (character.state == VN_Character.State.active && character.data != null)
            {
				if (character == speaker)
				{
					StartCoroutine(character.TweenColor(character.VN_CharSprite,
						speakingColor, speakerLightDuration, speakerLightEase));
				}
				else
				{
					StartCoroutine(character.TweenColor(character.VN_CharSprite,
						nonSpeakingColor, speakerLightDuration, speakerLightEase));
				}
			}
        }

		yield return null;
	}

	public IEnumerator ResetCharacters()
    {
		foreach (VN_Character charObj in CharacterObjects)
		{
			if (charObj.data != null)
			{
				// HACK Make new teleport to temp replace transition
				CharacterTransition originalTransition = charObj.data.transition;
				TeleportCharacterTransition tempTeleport =
					(TeleportCharacterTransition)ScriptableObject.CreateInstance(typeof(TeleportCharacterTransition));
				charObj.data.transition = tempTeleport;

				// Do same thing as ExitPart
				yield return StartCoroutine(charObj.data.transition
					.Co_ExitScreen(charObj, this));
				charObj.data.transition = originalTransition;
				charObj.ChangeSprite("");
				charObj.SetData(null);

				Destroy(tempTeleport);
			}
		}
	}
}