using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    public GameObject glassBreakSound;
    public GameObject conusGamesSound;
    public Camera mainCamera;
    void Awake()
    {
        Application.targetFrameRate = 60; //залочить до 60 фпс
        QualitySettings.vSyncCount = 0; //теперь фпс не зависит от монитора
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Invoke(nameof(LoadGlassBreakSound), 0.35f);
    }
    void LoadGlassBreakSound()
    {
        GameObject glassBreakSoundClone = Instantiate(glassBreakSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = glassBreakSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(glassBreakSoundClone, audioLength);
        Invoke(nameof(LoadConusGamesSound), audioLength - 1.5f);
    }
    void LoadConusGamesSound()
    {
        GameObject conusGamesSoundClone = Instantiate(conusGamesSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = conusGamesSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(conusGamesSoundClone, audioLength + 0.5f); //дать дурачку договорить
        Invoke(nameof(LoadScene), audioLength + 2f);
    }
    void LoadScene()
    {
        SceneManager.LoadScene("Loading");
    }
}