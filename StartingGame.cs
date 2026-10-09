using UnityEngine;

public class StartingGame : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject backgroundOutsideSound;
    public GameObject beginSpeechSound;
    public GameObject boxShakingSound;
    public GameObject[] bottlesToDrop;
    public GameObject[] bottlesToDropSound;
    public GameObject currentBox;
    public GameObject newBox;
    public GameObject instructionsSound;
    public bool isGameStarted = false;
    void Start()
    {
        LoadBackgroundOutsideSound();
    }
    void LoadBackgroundOutsideSound()
    {
        GameObject backgroundOutsideSoundClone = Instantiate(backgroundOutsideSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = backgroundOutsideSoundClone.GetComponent<AudioSource>();
        audio.Play();
        Invoke(nameof(LoadBeginSpeechSound), 3f);
    }
    void LoadBeginSpeechSound()
    {
        GameObject beginSpeechSoundClone = Instantiate(beginSpeechSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = beginSpeechSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(beginSpeechSoundClone, audioLength + 0.5f); //дать дурачку договорить
        Invoke(nameof(LoadBoxShakingSound), 6f);
        Invoke(nameof(LoadBottlesToDrop), 7.5f);
        Invoke(nameof(LoadBottlesToDropSound), 6.75f);
    }
    void LoadBoxShakingSound()
    {
        GameObject boxShakingSoundClone = Instantiate(boxShakingSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = boxShakingSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(boxShakingSoundClone, audioLength);
    }
    void LoadBottlesToDrop()
    {
        for (int i = 0; i < bottlesToDrop.Length; i++)
        {
            GameObject bottleToDropClone = Instantiate(bottlesToDrop[i], new Vector3(1.0378f, 2.303f, -6.588f), Quaternion.Euler(-42.634f, 1.24f, -90.28f));
            Destroy(bottleToDropClone, 5f);
        }
        for (int i = 0; i < bottlesToDrop.Length; i++)
        {
            GameObject bottleToDropClone = Instantiate(bottlesToDrop[i], new Vector3(1.0378f, 2.303f, -6.588f), Quaternion.Euler(-42.634f, 1.24f, -90.28f));
            Destroy(bottleToDropClone, 5f);
        }
    }
    void LoadBottlesToDropSound()
    {
        for (int i = 0; i < bottlesToDropSound.Length; i++)
        {
            GameObject bottleToDropSoundClone = Instantiate(bottlesToDropSound[i], mainCamera.transform.position, Quaternion.identity);
            AudioSource audio = bottleToDropSoundClone.GetComponent<AudioSource>();
            audio.PlayDelayed(Random.Range(1f, 3f));
            float audioLength = audio.clip.length;
            Destroy(bottleToDropSoundClone, audioLength + 3f);
        }
        for (int i = 0; i < bottlesToDropSound.Length; i++)
        {
            GameObject bottleToDropSoundClone = Instantiate(bottlesToDropSound[i], mainCamera.transform.position, Quaternion.identity);
            AudioSource audio = bottleToDropSoundClone.GetComponent<AudioSource>();
            audio.PlayDelayed(Random.Range(1f, 3f));
            float audioLength = audio.clip.length;
            Destroy(bottleToDropSoundClone, audioLength + 3f);
        }
        Invoke(nameof(DeleteCurrentBox), 2f);
        Invoke(nameof(LoadNewBox), 2f);
    }
    void DeleteCurrentBox()
    {
        Destroy(currentBox);
    }
    void LoadNewBox()
    {
        GameObject newBoxClone = Instantiate(newBox, new Vector3(0.692f, 2.589998f, -8.337f), Quaternion.Euler(0f, 180f, 180f));
        Animator animator = newBoxClone.GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(animator);
        }
        Invoke(nameof(UnlockCursor), 3f);
        Invoke(nameof(LoadInstructionsSound), 4f);
    }
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void LoadInstructionsSound()
    {
        GameObject instructionsSoundClone = Instantiate(instructionsSound, mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = instructionsSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(instructionsSoundClone, audioLength + 0.5f); //дать дурачку договорить
        //окончательное приготовление к игре
        isGameStarted = true;
        Animator animator = mainCamera.GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(animator);
        }
    }
}
