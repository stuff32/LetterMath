﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class LetterMathModule : MonoBehaviour
{

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Buttons;
    public TextMesh[] ButtonTexts;
    public TextMesh ScreenText;

    private readonly string letters = "_ABCDEFGHIJ";
    private int[] characters = new int[2];
    private bool _operator; //False = -, true = +
    private int answer;
    private int[] buttonText = new int[3];
    private int correctButton;


    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        for (int i = 0; i < 2; i++)
            characters[i] = Rnd.Range(1, letters.Length);

        _operator = Rnd.Range(0, 2) == 0;

        if (_operator)
            answer = characters[0] + characters[1];
        else
            answer = characters[0] - characters[1];

        correctButton = Rnd.Range(0, Buttons.Length);

        for (int i = 0; i < Buttons.Length; i++)
            buttonText[i] = answer;
        for (int i = 0; i < Buttons.Length; i++)
            while (i != correctButton && (buttonText[i] == answer || Enumerable.Range(0, 3).Any(x => x != i && buttonText[x] == buttonText[i])))
                buttonText[i] = Rnd.Range(-9, 21);


        
    }

    // Use this for initialization
    void Start()
    {
        for (int btn = 0; btn < Buttons.Length; btn++)
        {
            Buttons[btn].OnInteract = ButtonPressed(btn);
            ButtonTexts[btn].text = buttonText[btn].ToString();
        }
            
        ScreenText.text = letters[characters[0]].ToString() + (_operator ? " + " : " - ") + letters[characters[1]].ToString();

        Log("The display is {0}", ScreenText.text);
        Log("The correct answer that has been generated is {0}", answer);
    }

    private KMSelectable.OnInteractHandler ButtonPressed(int btn)
    {
        return delegate
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[btn].transform);
            Buttons[btn].AddInteractionPunch();
            if (btn == correctButton)
            {
                Module.HandlePass();
                Log("Correct button has been pressed. Module solved!");
            }
            else
            {
                Module.HandleStrike();
                Log("Incorrect button has been pressed. You have pressed {0}. I was expecting {1}.", buttonText[btn], answer);
            }
            return false;

        };
    }

    private void Log(string message, params object[] args)
    {
        Debug.LogFormat("[Letter Math #{0}] {1}", _moduleID, string.Format(message, args));
        
    }
    
}