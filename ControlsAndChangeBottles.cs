using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class ControlsAndChangeBottles : MonoBehaviour //тут вайбкодинг с управлением ну блин, забей
{
    public StartingGame startingGame; //это скрипт
    public Manager manager; //это еще один скрипт
    #region ”правление камерой
    public float sensitivity = 60f;
    float targetPitchX, targetYawY;
    float currentPitchX, currentYawY;
    float velPitchX, velYawY;
    #endregion
    public GameObject[] allBottlesArray;
    List<GameObject> hideBottles = new List<GameObject>();
    List<GameObject> ourBottles = new List<GameObject>();
    List<GameObject> forms = new List<GameObject>();
    public GameObject form;
    public GameObject bottlePlantSound;
    public int amountBottlesForGame = 4;
    bool isSpawnedHideBottles = false;
    bool isSpawnedOurBottles = false;
    bool isSpawnedForms = false;
    public bool isBottleTakenLeftHand = false;
    public bool isBottleTakenRightHand = false;
    public int countTakenBottles = 0; //чтобы не вз€ли лишнюю бутылку
    public int countPlantedBottles = 0; //чтобы воврем€ вызвать проверку как поставили бутылки
    string pattern = @"^bottle\d*\(Clone\)$"; //^-начало строки; слово; \d+-одно или больше чисел, а вот \d*-нуль или больше чисел; экранизаци€ скобок; $-конец строки
    public bool isNeedToReact = false;
    public GameObject correctOrNotPlantedBottle = null;
    void Start()
    {
        #region ”правление камерой
        if (startingGame != null && startingGame.mainCamera != null)
        {
            Vector3 e = startingGame.mainCamera.transform.rotation.eulerAngles;
            targetPitchX = currentPitchX = NormalizeAngle(e.x);
            targetYawY = currentYawY = NormalizeAngle(e.y);
        }
        #endregion
    }

    void Update()
    {
        #region ”правление камерой
        if (!startingGame.isGameStarted || startingGame == null || startingGame.mainCamera == null) return;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        targetYawY += mouseX * sensitivity * Time.deltaTime;
        targetPitchX -= mouseY * sensitivity * Time.deltaTime;
        targetYawY = Mathf.Clamp(targetYawY, -40f, 40f);
        targetPitchX = Mathf.Clamp(targetPitchX, -40f, 40f);
        currentYawY = Mathf.SmoothDamp(currentYawY, targetYawY, ref velYawY, 0.08f);
        currentPitchX = Mathf.SmoothDamp(currentPitchX, targetPitchX, ref velPitchX, 0.08f);
        startingGame.mainCamera.transform.rotation = Quaternion.Euler(currentPitchX, currentYawY, 0f);
        #endregion
        if (startingGame.isGameStarted && !isSpawnedHideBottles)
        {
            isSpawnedHideBottles = true;
            hideBottles = allBottlesArray.OrderBy(bottle => Random.value).Take(amountBottlesForGame).ToList();
            ourBottles = hideBottles.OrderBy(bottle => System.Array.IndexOf(allBottlesArray, bottle)).ToList();
            SpawnHideBottles();
        }
        if (startingGame.isGameStarted && !isSpawnedOurBottles)
        {
            isSpawnedOurBottles = true;
            SpawnOurBottles();
            manager.text.enabled = true;
            manager.text.color = Color.white;
            manager.text.text = "Ѕ”ƒ№“≈ ¬Ќ»ћј“≈Ћ№Ќџ! »√–ј Ќј„јЋј—№! —Ћ”Ўј≈ћ...";
            Invoke(nameof(ResetText), 3f);
        }
        if (Input.GetMouseButtonDown(0)) //Ћ ћ
        {
            if (manager.IsFoolSpeaking()) return;
            if (manager.isTheEnd) return;
            Ray ray = startingGame.mainCamera.ScreenPointToRay(Input.mousePosition); //луч
            RaycastHit hit; //попадание
            if (Physics.Raycast(ray, out hit)) //выпустить луч и получить попадание
            {
                GameObject clickedObject = hit.collider.gameObject.transform.parent.gameObject;
                if (clickedObject.tag == "CorrectOrNotPlanted") //если это та бутылка, где пользователь думает верно поставлена
                {
                    correctOrNotPlantedBottle = clickedObject;
                }
                else if (Regex.IsMatch(clickedObject.name, pattern) && !isBottleTakenLeftHand) //если это бутылка...
                {
                    BottleInfo bottleInfo = clickedObject.GetComponent<BottleInfo>();
                    if (bottleInfo.position == 0 && countTakenBottles < 2) //бутылка не на (форме) коробке... » кол-во вз€тых бутылок меньше 2
                    {
                        isBottleTakenLeftHand = true;
                        TakeBottleLeftHand(clickedObject);
                    }
                }
                else if (clickedObject.name == "form(Clone)" && isBottleTakenLeftHand) //если это форма...
                {
                    PlantBottleLeftHand(clickedObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) //ѕ ћ
        {
            if (manager.IsFoolSpeaking()) return;
            if (manager.isTheEnd) return;
            Ray ray = startingGame.mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject.transform.parent.gameObject;
                if (Regex.IsMatch(clickedObject.name, pattern) && !isBottleTakenRightHand) //если это бутылка...
                {
                    BottleInfo bottleInfo = clickedObject.GetComponent<BottleInfo>();
                    if (bottleInfo.position == 0 && countTakenBottles < 2) //бутылка не на (форме) коробке... » кол-во вз€тых бутылок меньше 2
                    {
                        isBottleTakenRightHand = true;
                        TakeBottleRightHand(clickedObject);
                    }
                }
                else if (clickedObject.name == "form(Clone)" && isBottleTakenRightHand) //если это форма...
                {
                    PlantBottleRightHand(clickedObject);
                }
            }
        }
        if ((isBottleTakenLeftHand || isBottleTakenRightHand) && !isSpawnedForms)
        {
            isSpawnedForms = true;
            SpawnForms();
        }
        else if ((isBottleTakenLeftHand || isBottleTakenRightHand) && isSpawnedForms)
        {
            GameObject[] plantedBottles = GameObject.FindGameObjectsWithTag("Planted");
            GameObject[] correctPlantedBottles = GameObject.FindGameObjectsWithTag("CorrectPlanted");
            foreach (var form in forms)
            {
                FormInfo formInfo = form.GetComponent<FormInfo>();
                bool isFormBusy = false;
                foreach (var plantedBottle in plantedBottles)
                {
                    if (plantedBottle.GetComponent<BottleInfo>().position == formInfo.position)
                    {
                        isFormBusy = true;
                        break;
                    }
                }
                if (!isFormBusy)
                {
                    foreach (var correctPlantedBottle in correctPlantedBottles)
                    {
                        if (correctPlantedBottle.GetComponent<BottleInfo>().position == formInfo.position)
                        {
                            isFormBusy = true;
                            break;
                        }
                    }
                }
                form.SetActive(!isFormBusy && (isBottleTakenLeftHand || isBottleTakenRightHand));
            }
        }
        if (countPlantedBottles == 2 && !isNeedToReact)
        {
            isNeedToReact = true;
        }
    }
    private static float NormalizeAngle(float a) //”правление камерой
    {
        return (a > 180f) ? a - 360f : a;
    } 
    void SpawnHideBottles()
    {
        if (amountBottlesForGame == 4)
        {
            GameObject hideBottle = Instantiate(hideBottles[0], new Vector3(0.1782888f, 1.789998f, -7.806f), Quaternion.identity);
            BottleInfo hideBottleInfo = hideBottle.AddComponent<BottleInfo>();
            hideBottleInfo.position = 1;
            GameObject hideBottle2 = Instantiate(hideBottles[1], new Vector3(0.513f, 1.789998f, -7.806f), Quaternion.identity);
            BottleInfo hideBottle2Info = hideBottle2.AddComponent<BottleInfo>();
            hideBottle2Info.position = 2;
            GameObject hideBottle3 = Instantiate(hideBottles[2], new Vector3(0.843f, 1.789998f, -7.806f), Quaternion.identity);
            BottleInfo hideBottle3Info = hideBottle3.AddComponent<BottleInfo>();
            hideBottle3Info.position = 3;
            GameObject hideBottle4 = Instantiate(hideBottles[3], new Vector3(1.135f, 1.789998f, -7.806f), Quaternion.identity);
            BottleInfo hideBottle4Info = hideBottle4.AddComponent<BottleInfo>();
            hideBottle4Info.position = 4;
        }
        //if если больше, то другие координаты и т.п.
    }
    void SpawnOurBottles()
    {
        if (amountBottlesForGame == 4)
        {
            GameObject ourBottle = Instantiate(ourBottles[0], new Vector3(0.1782888f, 1.789998f, -8.921255f), Quaternion.identity);
            BottleInfo ourBottleInfo = ourBottle.AddComponent<BottleInfo>();
            ourBottleInfo.position = 0;
            ourBottleInfo.ourPosition = 1;
            GameObject ourBottle2 = Instantiate(ourBottles[1], new Vector3(0.513f, 1.789998f, -8.921255f), Quaternion.identity);
            BottleInfo ourBottle2Info = ourBottle2.AddComponent<BottleInfo>();
            ourBottle2Info.position = 0;
            ourBottle2Info.ourPosition = 2;
            GameObject ourBottle3 = Instantiate(ourBottles[2], new Vector3(0.843f, 1.789998f, -8.921255f), Quaternion.identity);
            BottleInfo ourBottle3Info = ourBottle3.AddComponent<BottleInfo>();
            ourBottle3Info.position = 0;
            ourBottle3Info.ourPosition = 3;
            GameObject ourBottle4 = Instantiate(ourBottles[3], new Vector3(1.135f, 1.789998f, -8.921255f), Quaternion.identity);
            BottleInfo ourBottle4Info = ourBottle4.AddComponent<BottleInfo>();
            ourBottle4Info.position = 0;
            ourBottle4Info.ourPosition = 4;
        }
        //if если больше, то другие координаты и т.п.
    }
    void TakeBottleLeftHand(GameObject bottleLeftHand)
    {
        countTakenBottles += 1;
        GameObject takeBottleSoundClone = Instantiate(startingGame.bottlesToDropSound[Random.Range(0, startingGame.bottlesToDropSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = takeBottleSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(takeBottleSoundClone, audioLength);
        bottleLeftHand.tag = "BottleLeftHand";
        bottleLeftHand.transform.SetParent(startingGame.mainCamera.transform);
        bottleLeftHand.transform.localPosition = new Vector3(-0.75f, -0.45f, 1f);
        bottleLeftHand.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
    }
    void TakeBottleRightHand(GameObject bottleRightHand)
    {
        countTakenBottles += 1;
        GameObject takeBottleSoundClone = Instantiate(startingGame.bottlesToDropSound[Random.Range(0, startingGame.bottlesToDropSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = takeBottleSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(takeBottleSoundClone, audioLength);
        bottleRightHand.tag = "BottleRightHand";
        bottleRightHand.transform.SetParent(startingGame.mainCamera.transform);
        bottleRightHand.transform.localPosition = new Vector3(0.75f, -0.45f, 1f);
        bottleRightHand.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
    }
    void SpawnForms()
    {
        if (amountBottlesForGame == 4)
        {
            GameObject formClone = Instantiate(form, new Vector3(0.217f, 2.589998f, -8.327f), Quaternion.identity);
            FormInfo formCloneInfo = formClone.AddComponent<FormInfo>();
            formCloneInfo.position = 1;
            forms.Add(formClone);
            GameObject form2Clone = Instantiate(form, new Vector3(0.533f, 2.589998f, -8.327f), Quaternion.identity);
            FormInfo form2CloneInfo = form2Clone.AddComponent<FormInfo>();
            form2CloneInfo.position = 2;
            forms.Add(form2Clone);
            GameObject form3Clone = Instantiate(form, new Vector3(0.838f, 2.589998f, -8.327f), Quaternion.identity);
            FormInfo form3CloneInfo = form3Clone.AddComponent<FormInfo>();
            form3CloneInfo.position = 3;
            forms.Add(form3Clone);
            GameObject form4Clone = Instantiate(form, new Vector3(1.143f, 2.589998f, -8.327f), Quaternion.identity);
            FormInfo form4CloneInfo = form4Clone.AddComponent<FormInfo>();
            form4CloneInfo.position = 4;
            forms.Add(form4Clone);
        }
        //if если больше, то другие координаты и т.п.
    }
    void PlantBottleLeftHand(GameObject clickedObject)
    {
        countPlantedBottles += 1;
        GameObject bottleLeftHand = GameObject.FindWithTag("BottleLeftHand");
        bottleLeftHand.tag = "Planted";
        bottleLeftHand.transform.SetParent(null);
        bottleLeftHand.transform.position = clickedObject.transform.position;
        bottleLeftHand.transform.rotation = Quaternion.identity;
        isBottleTakenLeftHand = false;
        GameObject bottlePlantSoundClone = Instantiate(bottlePlantSound, startingGame.mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = bottlePlantSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(bottlePlantSoundClone, audioLength);
        BottleInfo bottleInfo = bottleLeftHand.GetComponent<BottleInfo>();
        FormInfo formInfo = clickedObject.GetComponent<FormInfo>();
        bottleInfo.position = formInfo.position;
        clickedObject.SetActive(false);
        if (!isBottleTakenRightHand)
        {
            foreach (var form in forms)
            {
                form.SetActive(false);
            }
        }
    }
    void PlantBottleRightHand(GameObject clickedObject)
    {
        countPlantedBottles += 1;
        GameObject bottleRightHand = GameObject.FindWithTag("BottleRightHand");
        bottleRightHand.tag = "Planted";
        bottleRightHand.transform.SetParent(null);
        bottleRightHand.transform.position = clickedObject.transform.position;
        bottleRightHand.transform.rotation = Quaternion.identity;
        isBottleTakenRightHand = false;
        GameObject bottlePlantSoundClone = Instantiate(bottlePlantSound, startingGame.mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = bottlePlantSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(bottlePlantSoundClone, audioLength);
        BottleInfo bottleInfo = bottleRightHand.GetComponent<BottleInfo>();
        FormInfo formInfo = clickedObject.GetComponent<FormInfo>();
        bottleInfo.position = formInfo.position;
        clickedObject.SetActive(false);
        if (!isBottleTakenLeftHand)
        {
            foreach (var form in forms)
            {
                form.SetActive(false);
            }
        }
    }
    public void ResetText()
    {
        manager.text.enabled = false;
        manager.text.text = "";
        manager.text.color = Color.white;
    }
}
public class BottleInfo : MonoBehaviour //наследование от MonoBehaviour дает возможность добавление данного класса как компонент
{
    public int ourPosition; //чтобы вернуть бутылку к our на законное место
    public int position; //позици€ бутылки дл€ сравнени€ с hide
}
public class FormInfo : MonoBehaviour
{
    public int position; //будем передавать данную позицию к our bottle на коробке
}