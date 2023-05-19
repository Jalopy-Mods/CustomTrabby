using System;
using System.Collections.Generic;
using JaLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomTrabby
{
    public class CustomTrabby : Mod
    {
        public override string ModID => "CustomTrabby";
        public override string ModName => "Custom Trabby";
        public override string ModAuthor => "MeblIkea";
        public override string ModDescription => "Let you change the color of the laika, and the height of the suspension as well.";
        public override string ModVersion => "1.0.1";

        public override WhenToInit WhenToInit => WhenToInit.InGame; // OR WhenToInit.InMenu (In menu is both)

        private bool _menuState;
        private MonoBehaviour _gui;
        private MouseLook _locker0;
        private MouseLook _locker1;

        public override void Start()
        {
            _locker0 = GameObject.Find("/First Person Controller").GetComponent<MouseLook>();
            _locker1 = GameObject.Find("/First Person Controller/Main Camera").GetComponent<MouseLook>();
            _gui = new GameObject("customizer_gui").AddComponent<ModGUI>();
            _gui.GetComponent<ModGUI>().hidden = true;
        }
        private void ToggleMenu()
        {
            if (!SceneManager.GetActiveScene().name.Equals("Scn2_CheckpointBravo")) return;
            if (!_menuState)
            {
                _menuState = true;
                _gui.GetComponent<ModGUI>().hidden = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _locker0.enabled = false;
                _locker1.enabled = false;
            }
            else
            {
                _menuState = false;
                _gui.GetComponent<ModGUI>().hidden = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                _locker0.enabled = true;
                _locker1.enabled = true;
            }
        }
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleMenu();
            }
            if (_menuState)
            {
                UpdateCursorControl(true);
            }
        }

        private static void UpdateCursorControl(bool locked)
        {
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
   
    public class ModGUI : MonoBehaviour
    {
        public Material trabby;
        public string[] trabbyRGB;
        public Material lDoor;
        public string[] lDoorRGB;
        public Material rDoor;
        public string[] rDoorRGB;
        public Material hood;
        public string[] hoodRGB;
        public Material trunk;
        public string[] trunkRGB;
        public Material roof;
        public string[] roofRGB;
        private float _lhSliderValue;
        private GameObject _frSuspension;
        private GameObject _flSuspension;
        private GameObject _rrSuspension;
        private GameObject _rlSuspension;
        private EngineComponentC _fuelTank;
        public CarPerformanceC enginePerformance;
        public CarControleScriptC engineControl;

        private bool _infiniteFuel;
        public float hSliderValue;
        private Dictionary<string, bool> _rainbowed;
        public bool hidden;

        private bool _ready;
        private void Awake()
        {
            trabby = GameObject.Find("/FrameHolder/TweenHolder/Frame").GetComponent<MeshRenderer>().material;
            trabbyRGB = gen_rgb(trabby);
            hood = GameObject.Find("/FrameHolder/TweenHolder/Frame/Boot").GetComponent<MeshRenderer>().material;
            hoodRGB = gen_rgb(hood);
            trunk = GameObject.Find("/FrameHolder/TweenHolder/Frame/Bonnet").GetComponent<MeshRenderer>().material;
            trunkRGB = gen_rgb(trunk);
            roof = GameObject.Find("/FrameHolder/TweenHolder/Frame/Roof").GetComponent<MeshRenderer>().material;
            roofRGB = gen_rgb(roof);
            lDoor = GameObject.Find("/FrameHolder/TweenHolder/Frame/L_Door").GetComponent<MeshRenderer>().material;
            lDoorRGB = gen_rgb(lDoor);
            
            _frSuspension = GameObject.Find("FrameHolder/Wheel_Colliders/FL_Wheel");
            _flSuspension = GameObject.Find("FrameHolder/Wheel_Colliders/FR_Wheel");
            _rrSuspension = GameObject.Find("FrameHolder/Wheel_Colliders/RL_Wheel");
            _rlSuspension = GameObject.Find("FrameHolder/Wheel_Colliders/RR_Wheel");
            
            engineControl = GameObject.Find("FrameHolder").GetComponent<CarControleScriptC>();
            enginePerformance = GameObject.Find("FrameHolder/TweenHolder/Frame").GetComponent<CarPerformanceC>();

            _ready = false;
            _rainbowed = new Dictionary<string, bool>
            {
                { "Frame", false },
                { "Left Door", false },
                { "Right Door", false },
                { "Hood", false },
                { "Trunk", false },
                { "Roof", false }
            };
        }

        private static string[] gen_rgb(Material material)
        {
            return new []{Math.Round(material.color.r*255) + "", Math.Round(material.color.g*255) + "", Math.Round(material.color.b * 255) + ""};
        }
        public void OnGUI()
        {
            if (_ready)
            {
                if (!hidden) {
                    GUI.Box(new Rect(5, 5, 320, 500), "<b>Customizer</b>");
                    
                    GUI.Label(new Rect(10, 320, 100, 20), "Vehicle height");
                    hSliderValue = GUI.HorizontalSlider(new Rect(10, 335, 100, 30), hSliderValue, 1.0F, 3.0F);
                    if (Math.Abs(hSliderValue - _lhSliderValue) > 0.01)
                    {
                        _lhSliderValue = hSliderValue;
                        _frSuspension.transform.localScale = new Vector3(hSliderValue, hSliderValue, hSliderValue);
                        _flSuspension.transform.localScale = new Vector3(hSliderValue, hSliderValue, hSliderValue);
                        _rlSuspension.transform.localScale = new Vector3(hSliderValue, hSliderValue, hSliderValue);
                        _rrSuspension.transform.localScale = new Vector3(hSliderValue, hSliderValue, hSliderValue);
                    }
                    
                    GUI.Label(new Rect(10, 365, 100, 20), "Engine power");
                    if (GUI.Button(new Rect(10, 385, 120, 20), "SCHNELLL!!!!"))
                    {
                        enginePerformance.actualTopSpeed = 500;
                        engineControl.lowSpeedTorqueMultiplier = 5000;
                        engineControl.lowSpeedTorqueChangeOver = 50_000;
                        engineControl.maxTorque = 100_000;
                    }
                    
                    GUI.Label(new Rect(10, 420, 100, 20), "Unlimited fuel");
                    _infiniteFuel = GUI.Toggle(new Rect(10, 435, 150, 20), _infiniteFuel, "Infinite Fuel");

                    if (GUI.Button(new Rect(320 - 100, 320, 100, 20), "jeb_ mode"))
                    {
                        _rainbowed["Frame"] = true;
                        _rainbowed["Left Door"] = true;
                        _rainbowed["Right Door"] = true;
                        _rainbowed["Hood"] = true;
                        _rainbowed["Trunk"] = true;
                        _rainbowed["Roof"] = true;
                        
                        trabby.color = Color.blue;
                        lDoor.color = Color.blue;
                        rDoor.color = Color.blue;
                        hood.color = Color.blue;
                        trunk.color = Color.blue;
                        roof.color = Color.blue;
                    }
                }
                ElementColor(trabby, trabbyRGB, "Frame", 0);
                ElementColor(lDoor, lDoorRGB, "Left Door", 1);
                ElementColor(rDoor, rDoorRGB, "Right Door", 2);
                ElementColor(hood, hoodRGB, "Hood", 3);
                ElementColor(trunk, trunkRGB, "Trunk", 4);
                ElementColor(roof, roofRGB, "Roof", 5);
                if (_infiniteFuel)
                {
                    _fuelTank.currentFuelAmount = 100;
                }
            }
            else
            {
                GUI.Box(new Rect(5, 5, 100, 20), "Not ready yet");
                if (!GameObject.Find("FrameHolder/TweenHolder/Frame/EngineHolders/TankHolder/FuelTank"))
                {
                    _fuelTank = GameObject.Find("FrameHolder/TweenHolder/Frame/EngineHolders/TankHolder/FuelTank").GetComponent<EngineComponentC>();
                }
                if (GameObject.Find("/FrameHolder/TweenHolder/Frame/DoorHolder/R_Door") is null) return;
                rDoor = GameObject.Find("/FrameHolder/TweenHolder/Frame/DoorHolder/R_Door")
                    .GetComponent<MeshRenderer>().material;
                rDoorRGB = gen_rgb(rDoor);
                _ready = true;
            }
        }

        private static void Corrector(IList<string> before)
        {
            for (var i = 0; i<3; i++)
            {
                int.TryParse(before[i], out var res);
                if (res > 255)
                {
                    res = 255;
                }
                before[i] = res.ToString();
            }
        }
        public void ElementColor(Material element, string[] rgb, string componentName, int offset)
        {
            var chosenColor = new Color(float.Parse(rgb[0])/255, float.Parse(rgb[1])/255, float.Parse(rgb[2])/255);
            var style = new GUIStyle {normal = {textColor = chosenColor}};
            element.color = chosenColor;
            
            if (!hidden)
            {
                GUI.Label(new Rect(10, 50 + 45 * offset, 100, 20), componentName, style);
                rgb[0] = GUI.TextField(new Rect(10, 65 + 45 * offset, 30, 20), rgb[0], 3);
                rgb[1] = GUI.TextField(new Rect(40, 65 + 45 * offset, 30, 20), rgb[1], 3);
                rgb[2] = GUI.TextField(new Rect(70, 65 + 45 * offset, 30, 20), rgb[2], 3);
                Corrector(rgb);

                _rainbowed[componentName] = GUI.Toggle(new Rect(105, 65 + 45 * offset, 100, 20),
                    _rainbowed[componentName], "Rainbow mode");
            }

            if (!_rainbowed[componentName]) return;
            if (int.Parse(rgb[0]) != 255 & int.Parse(rgb[1]) != 255 & int.Parse(rgb[2]) != 255)
            {
                rgb[0] = "255";
            } else if (int.Parse(rgb[0]) == 255 & int.Parse(rgb[1]) != 255)
            {
                rgb[1] = int.Parse(rgb[1])+1 + "";
                rgb[2] = int.Parse(rgb[2])-1 + "";
            } else if (int.Parse(rgb[1]) == 255 & int.Parse(rgb[2]) != 255)
            {
                rgb[2] = int.Parse(rgb[2])+1 + "";
                rgb[0] = int.Parse(rgb[0])-1 + "";
            } else if (int.Parse(rgb[2]) == 255 & int.Parse(rgb[0]) != 255)
            {
                rgb[0] = int.Parse(rgb[0])+1 + "";
                rgb[1] = int.Parse(rgb[1])-1 + "";
            }
            Corrector(rgb);
            // GUI.Toggle(new Rect (210, 65+45*offset, 100, 20), "Uncle texture");
        }
    }
}