
using UnityEngine;
using UnityEngine.InputSystem;
using Assista.UnitTools;
using System.Collections;


public class CharacterInputSystem : SingletonBase<CharacterInputSystem>
{
    //public Button button;
    public PlayerControls _playerControls;
    //属性
    public Vector2 playerMovement
    {
        //WASD
        get => _playerControls.PlayerInput.Movement.ReadValue<Vector2>();
        
    }

    public float CameraDistance
    {
        //鼠标滚轮
        get => Mouse.current.scroll.y.ReadValue();

    }

    public Vector2 cameraLook
    {
        //鼠标移动
        get => _playerControls.PlayerInput.CameraLook.ReadValue<Vector2>();

    }

    public bool playerRun
    {
        //???
        get => _playerControls.PlayerInput.Run.phase == InputActionPhase.Performed;

    }

    private bool _playerDefen;
    public bool playerDefen
    {
        //点按鼠标右键
        get => _playerControls.PlayerInput.Defen.triggered || _playerDefen;
    }
    public void Set_playerDefen()
    {
        _playerDefen = true;
        //启动协程，一定时间后修改_Combat_E的值
        StartCoroutine(_playerDefenCoroutine());
    }
    //协程
    IEnumerator _playerDefenCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _playerDefen = false;
    }

    //public bool playerDefenLong
    //{
    //    //长按鼠标右键
    //    get => _playerControls.PlayerInput.DefenLong.phase == InputActionPhase.Performed;
    //}

    private bool _playerJump;
    public bool playerJump
    {
        //点按空格
        get => _playerControls.PlayerInput.Jump.triggered || _playerJump;

    }
    public void SetplayerJump()
    {
        _playerJump = true;
        //StartCoroutine(playerJumpCoroutine());
    }

    //协程
    IEnumerator playerJumpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _playerJump = false;
    }

    //public bool playerWeaponAnimation
    //{
    //    //点按R
    //    get => _playerControls.PlayerInput.WeaponAnimation.triggered;

    //}

    public bool Combat_E_Long
    {
        get => _playerControls.PlayerInput.Combat_E_Long.phase == InputActionPhase.Performed;
    }

    private bool _Combat_E;
    public bool Combat_E
    {
        //点按E
        get => _playerControls.PlayerInput.Combat_E.triggered || _Combat_E;

    }
    public void SetCombat_E()
    {
        _Combat_E = true;
        //启动协程，一定时间后修改_Combat_E的值
        StartCoroutine(Combat_ECoroutine());
    }
    //协程
    IEnumerator Combat_ECoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_E = false;
    }

    private float combatFHoldTime = 0f; // 持续按下时间
    private float requiredHoldTime = 0.5f; // 长按所需时间

    public bool Combat_F_Long
    {
        get => _playerControls.PlayerInput.Combat_F_Long.phase == InputActionPhase.Performed;
    }

    //public bool Combat_F_Long
    //{
    //    get => combatFHoldTime >= requiredHoldTime;
    //}

    private bool _Combat_F;
    public bool Combat_F
    {
        //点按F
        get => _playerControls.PlayerInput.Combat_F.triggered || _Combat_F;

    }
    public void SetCombat_F()
    {
        _Combat_F = true;
        //启动协程，一定时间后修改_Combat_F的值
        StartCoroutine(Combat_FCoroutine());
    }
    //协程
    IEnumerator Combat_FCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_F = false;
    }

    private bool _Combat_Q;
    public bool Combat_Q
    {
        //点按E
        get => _playerControls.PlayerInput.Combat_Q.triggered || _Combat_Q;

    }
    public void SetCombat_Q()
    {
        _Combat_Q = true;
        //启动协程，一定时间后修改_Combat_E的值
        StartCoroutine(Combat_QCoroutine());
    }
    //协程
    IEnumerator Combat_QCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_Q = false;
    }


    public int playerSwitchType
    {
        //左边数字1 2 3 4
        get => (int)_playerControls.PlayerInput.SwitchCharacter.ReadValue<float>();

    }

    public bool playerSlide
    {
        //点按左shift
        get => _playerControls.PlayerInput.Slide.triggered;

    }

    //public bool LeftAltLong
    //{
    //    //长按左alt
    //    get => _playerControls.PlayerInput.Left_Alt_Long.phase == InputActionPhase.Performed;
    //}


    public bool playerLAtkLong
    {
        get => _playerControls.PlayerInput.LAtk_Long.phase == InputActionPhase.Performed;
    }


    private bool _playerLAtk;
    private bool disablePlayerLAtk;
    public bool playerLAtk
    {
        //点按鼠标左键
        get => (_playerControls.PlayerInput.LAtk.triggered ) || _playerLAtk;
        //&& disablePlayerLAtk
    }
    /// <summary>
    /// 禁用鼠标左键攻击
    /// </summary>
    public void DisablePlayerLAtk() => disablePlayerLAtk = false;
    /// <summary>
    /// 启用鼠标左键攻击
    /// </summary>
    public void EnablePlayerLAtk() => disablePlayerLAtk = true;


    public void SetPlayerLAtk()
    {
        _playerLAtk = true;
        //启动协程，一定时间后修改_Combat_E的值
        StartCoroutine(PlayerLAtkCoroutine());
    }
    //协程
    IEnumerator PlayerLAtkCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _playerLAtk = false;
    }



    //内部函数
    private void Awake()
    {
        if (_playerControls == null)
            _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void OnDisable()
    {
        _playerControls.Disable();
    }
    private void Update()
    {
        if (_playerControls.PlayerInput.Combat_F.ReadValue<float>() > 0) // 如果按键被按下
        {
            combatFHoldTime += Time.deltaTime; // 累加持续时间
        }
        else
        {
            combatFHoldTime = 0f; // 按键松开时重置计时
        }
    }
}
