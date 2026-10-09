using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public ControlsAndChangeBottles controlsScript; //это скрипт
    public StartingGame startingGame; //это скрипт тоже)
    public GameObject[] comeOnPlaySound;
    public GameObject[] cheeringSound;
    public GameObject[] notCheeringSound;
    public GameObject[] halfCheeringSound;
    public GameObject bottleReturnedBackSound;
    public GameObject theEndSound;
    public Animator foolAnimator;
    public Text text;
    int counterComeOnPlay = 0;
    bool isSayItComeOnPlay = false;
    bool isReacting = false;
    public bool isTheEnd = false;
    void Update()
    {
        if (startingGame.isGameStarted)
        {
            counterComeOnPlay++;
        }
        if (controlsScript.isBottleTakenLeftHand || controlsScript.isBottleTakenRightHand)
        {
            counterComeOnPlay = 0;
        }
        if (!isSayItComeOnPlay && counterComeOnPlay >= 60*75 && startingGame.isGameStarted) //60 кадров -> 1 секунда => 75 секунд
        {
            isSayItComeOnPlay = true;
            foolAnimator.SetBool("isComeOnPlay", true);
            GameObject comeOnPlaySoundClone = Instantiate(comeOnPlaySound[Random.Range(0, comeOnPlaySound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
            AudioSource audio = comeOnPlaySoundClone.GetComponent<AudioSource>();
            audio.Play();
            float audioLength = audio.clip.length;
            Destroy(comeOnPlaySoundClone, audioLength + 0.5f); //дать дурачку договорить
            Invoke(nameof(ResetIsComeOnPlay), 2f);
        }
        if (controlsScript.isNeedToReact && !isReacting)
        {
            isReacting = true;
            int hits = 0;
            GameObject firstPlantedBottle = null;
            GameObject secondPlantedBottle = null;
            GameObject[] plantedBottles = GameObject.FindGameObjectsWithTag("Planted");
            firstPlantedBottle = plantedBottles[0];
            secondPlantedBottle = plantedBottles[1];
            //идет перебор с тегом Planted бутылок их position с position hide бутылок и фиксируются
            GameObject[] firstPlantedBottleAndSameHideBottle = GameObject.FindObjectsOfType<GameObject>().Where(o => o.name == firstPlantedBottle.name).ToArray();
            BottleInfo firstPlantedBottleInfo = firstPlantedBottleAndSameHideBottle[0].GetComponent<BottleInfo>();
            BottleInfo hideBottleOfFirstPlantedBottleInfo = firstPlantedBottleAndSameHideBottle[1].GetComponent<BottleInfo>();
            if (firstPlantedBottleInfo.position == hideBottleOfFirstPlantedBottleInfo.position)
            {
                hits += 1;
            }
            GameObject[] secondPlantedBottleAndSameHideBottle = GameObject.FindObjectsOfType<GameObject>().Where(o => o.name == secondPlantedBottle.name).ToArray();
            BottleInfo secondPlantedBottleInfo = secondPlantedBottleAndSameHideBottle[0].GetComponent<BottleInfo>();
            BottleInfo hideBottleOfSecondPlantedBottleInfo = secondPlantedBottleAndSameHideBottle[1].GetComponent<BottleInfo>();
            if (secondPlantedBottleInfo.position == hideBottleOfSecondPlantedBottleInfo.position)
            {
                hits += 1;
            }
            //каждое попадание будет прибавлять int => максимум может быть 2 попадания


            //если int равен двум, то данные две эти зафиксированные бутылки отправляются в тег correctPlanted чтобы их нельзя было вернуть обратно к our бутылкам
            if (hits == 2)
            {
                text.enabled = true;
                text.text = "ВЕРНО! МОЛОДЕЦ, ВСЕ ВЕРНО";
                Invoke(nameof(ResetText), 2f);
                firstPlantedBottle.tag = "CorrectPlanted";
                secondPlantedBottle.tag = "CorrectPlanted";
                //анимка полюбэ даже если он angry переходит в верно и затем через hasExitTime вернется в idle
                foolAnimator.SetBool("isCheering", true);
                Invoke(nameof(ResetIsCheering), 1f);
                //дурачок хвалит игрока звуком
                GameObject cheeringSoundClone = Instantiate(cheeringSound[Random.Range(0, cheeringSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
                AudioSource audio = cheeringSoundClone.GetComponent<AudioSource>();
                audio.Play();
                float audioLength = audio.clip.length;
                Destroy(cheeringSoundClone, audioLength + 0.5f); //дать дурачку договорить
                //счетчик заплэнченных бутылок и взятых бутылок зануливается
                controlsScript.countPlantedBottles = 0;
                controlsScript.countTakenBottles = 0;
                //флажок isNeedToReact становится обратно в false
                controlsScript.isNeedToReact = false;
                isReacting = false;
            }
            else if (hits == 0)
            {
                text.enabled = true;
                text.color = Color.red;
                text.text = "ВСЕ МИМО! НЕ МОЛОДЕЦ, ПОДУМАЙТЕ ЛУЧШЕ";
                Invoke(nameof(ResetText), 2f);
                //если int равен нулю, то данные две эти зафиксированные бутылки отправляются в тег Untagged
                firstPlantedBottle.tag = "Untagged";
                secondPlantedBottle.tag = "Untagged";
                //и вернуть обратно их в our (вот как надо подумать и кстати надо position занулить крч тут через switch case проверят координаты в our и т.п
                switch (firstPlantedBottleInfo.ourPosition)
                {
                    case 1:
                        firstPlantedBottle.transform.position = new Vector3(0.1782888f, 1.789998f, -8.921255f);
                        break;
                    case 2:
                        firstPlantedBottle.transform.position = new Vector3(0.513f, 1.789998f, -8.921255f);
                        break;
                    case 3:
                        firstPlantedBottle.transform.position = new Vector3(0.843f, 1.789998f, -8.921255f);
                        break;
                    case 4:
                        firstPlantedBottle.transform.position = new Vector3(1.135f, 1.789998f, -8.921255f);
                        break;
                        //и так далее если будет больше позиций...
                }
                firstPlantedBottleInfo.position = 0;
                switch (secondPlantedBottleInfo.ourPosition)
                {
                    case 1:
                        secondPlantedBottle.transform.position = new Vector3(0.1782888f, 1.789998f, -8.921255f);
                        break;
                    case 2:
                        secondPlantedBottle.transform.position = new Vector3(0.513f, 1.789998f, -8.921255f);
                        break;
                    case 3:
                        secondPlantedBottle.transform.position = new Vector3(0.843f, 1.789998f, -8.921255f);
                        break;
                    case 4:
                        secondPlantedBottle.transform.position = new Vector3(1.135f, 1.789998f, -8.921255f);
                        break;
                        //и так далее если будет больше позиций...
                }
                secondPlantedBottleInfo.position = 0;
                //анимка полюбэ даже если он angry переходит в неверно и затем через hasExitTime вернется в idle
                foolAnimator.SetBool("isNotCheering", true);
                Invoke(nameof(ResetIsNotCheering), 1f);
                //дурачок ругает игрока звуком
                GameObject notCheeringSoundClone = Instantiate(notCheeringSound[Random.Range(0, notCheeringSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
                AudioSource audio = notCheeringSoundClone.GetComponent<AudioSource>();
                audio.Play();
                float audioLength = audio.clip.length;
                Destroy(notCheeringSoundClone, audioLength + 0.5f); //дать дурачку договорить
                GameObject bottleReturnedBackSoundClone = Instantiate(bottleReturnedBackSound, startingGame.mainCamera.transform.position, Quaternion.identity);
                AudioSource audio2 = bottleReturnedBackSoundClone.GetComponent<AudioSource>();
                audio2.Play();
                float audioLength2 = audio2.clip.length;
                Destroy(bottleReturnedBackSoundClone, audioLength2);
                //счетчик заплэнченных бутылок и взятых бутылок зануливается
                controlsScript.countPlantedBottles = 0;
                controlsScript.countTakenBottles = 0;
                //флажок isNeedToReact становится обратно в false
                controlsScript.isNeedToReact = false;
                isReacting = false;
            }
            else
            {
                text.enabled = true;
                text.text = "КОГДА УВАЖАЕМЫЙ ЧЕЛОВЕК ЗАМОЛЧИТ - ВЫБЕРИТЕ БУТЫЛКУ";
                Invoke(nameof(ResetText), 3f);
                //если int равен 1, то данные две эти зафиксированные бутылки отправляются в тег CorrectOrNotPlanted чтобы дальше лучом могли выбрать именно нужную нам бутылку
                firstPlantedBottle.tag = "CorrectOrNotPlanted";
                secondPlantedBottle.tag = "CorrectOrNotPlanted";
                //анимка полюбэ даже если он angry переходит в верноНаПоловину и затем через hasExitTime вернется в idle
                foolAnimator.SetBool("isNotCheering", true);
                Invoke(nameof(ResetIsNotCheering), 1f);
                //дурачок ругаетНаПоловину игрока звуком
                GameObject halfCheeringSoundClone = Instantiate(halfCheeringSound[Random.Range(0, halfCheeringSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
                AudioSource audio = halfCheeringSoundClone.GetComponent<AudioSource>();
                audio.Play();
                float audioLength = audio.clip.length;
                Destroy(halfCheeringSoundClone, audioLength + 0.5f); //дать дурачку договорить
                StartCoroutine(WaitingUntilUserSelectsBottleAndContinue()); //ожидание пока пользователь выберет по его мнению верную бутылку и ПРОДОЛЖЕНИЕ СКРИПТА...
            }
        }
        GameObject[] correctPlantedBottles = GameObject.FindGameObjectsWithTag("CorrectPlanted");
        if (controlsScript.amountBottlesForGame - correctPlantedBottles.Length <= 1)
        {
            isTheEnd = true;
            Invoke(nameof(LaunchTheEnd), 2f);
        }
    }
    void ResetIsComeOnPlay()
    {
        foolAnimator.SetBool("isComeOnPlay", false);
        counterComeOnPlay = 0;
        isSayItComeOnPlay = false;
    }
    void ResetIsCheering()
    {
        foolAnimator.SetBool("isCheering", false);
    }
    void ResetIsNotCheering()
    {
        foolAnimator.SetBool("isNotCheering", false);
    }
    IEnumerator WaitingUntilUserSelectsBottleAndContinue()
    {
        //создается луч и выпускается тогда и только тогда, когда мы попадаем на бутылку с тегом CorrectOrNotPlanted
        while (controlsScript.correctOrNotPlantedBottle == null)
        {
            yield return null;
        }
        //сразу после этого сравниваются position выбранной пользователем бутылки с position hide бутылки
        GameObject[] correctOrNotPlantedBottleAndSameHideBottle = GameObject.FindObjectsOfType<GameObject>().Where(o => o.name == controlsScript.correctOrNotPlantedBottle.name).ToArray();
        BottleInfo correctOrNotPlantedBottleInfo = correctOrNotPlantedBottleAndSameHideBottle[0].GetComponent<BottleInfo>();
        BottleInfo hideBottleOfCorrectOrNotPlantedBottleInfo = correctOrNotPlantedBottleAndSameHideBottle[1].GetComponent<BottleInfo>();
        if (correctOrNotPlantedBottleInfo.position == hideBottleOfCorrectOrNotPlantedBottleInfo.position)
        {
            text.enabled = true;
            text.text = "МОЛОДЕЦ, ВЕРНО! ВИДИМО ПОВЕЗЛО";
            Invoke(nameof(ResetText), 2f);
            //ТЕПЕРЬ ЕСЛИ ВЕРНО -> то данная выбранная бутылка отправляется в тег correctPlanted чтобы ее нельзя было вернуть обратно
            correctOrNotPlantedBottleAndSameHideBottle[0].tag = "CorrectPlanted";
            //другую же отправляем в тег Untagged и возвращаем ее в our и также зануливаем ее position
            GameObject plantedBottle = GameObject.FindGameObjectWithTag("CorrectOrNotPlanted");
            plantedBottle.tag = "Untagged";
            switch (plantedBottle.GetComponent<BottleInfo>().ourPosition)
            {
                case 1:
                    plantedBottle.transform.position = new Vector3(0.1782888f, 1.789998f, -8.921255f);
                    break;
                case 2:
                    plantedBottle.transform.position = new Vector3(0.513f, 1.789998f, -8.921255f);
                    break;
                case 3:
                    plantedBottle.transform.position = new Vector3(0.843f, 1.789998f, -8.921255f);
                    break;
                case 4:
                    plantedBottle.transform.position = new Vector3(1.135f, 1.789998f, -8.921255f);
                    break;
                    //и так далее если будет больше позиций...
            }
            plantedBottle.GetComponent<BottleInfo>().position = 0;
            //анимка полюбэ даже если он angry переходит в верно и затем через hasExitTime вернется в idle
            foolAnimator.SetBool("isCheering", true);
            Invoke(nameof(ResetIsCheering), 1f);
            //дурачок хвалит игрока звуком
            GameObject cheeringSoundClone = Instantiate(cheeringSound[Random.Range(0, cheeringSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
            AudioSource audio = cheeringSoundClone.GetComponent<AudioSource>();
            audio.Play();
            float audioLength = audio.clip.length;
            Destroy(cheeringSoundClone, audioLength + 0.5f); //дать дурачку договорить
            GameObject bottleReturnedBackSoundClone = Instantiate(bottleReturnedBackSound, startingGame.mainCamera.transform.position, Quaternion.identity);
            AudioSource audio2 = bottleReturnedBackSoundClone.GetComponent<AudioSource>();
            audio2.Play();
            float audioLength2 = audio2.clip.length;
            Destroy(bottleReturnedBackSoundClone, audioLength2);
        }
        else
        {
            text.enabled = true;
            text.color = Color.red;
            text.text = "НЕВЕРНО, КРИНЖ! СТЫД И ПОЗОР";
            Invoke(nameof(ResetText), 2f);
            //ТЕПЕРЬ ЕСЛИ НЕВЕРНО -> то данные две эти зафиксированные бутылки отправляются в тег Untagged и вернуть обратно их в our (вот как надо подумать и кстати надо position занулить крч тут через switch case проверят координаты в our и т.п
            GameObject[] plantedBottles = GameObject.FindGameObjectsWithTag("CorrectOrNotPlanted");
            GameObject firstPlantedBottle = plantedBottles[0];
            GameObject secondPlantedBottle = plantedBottles[1];
            firstPlantedBottle.tag = "Untagged";
            secondPlantedBottle.tag = "Untagged";
            switch (firstPlantedBottle.GetComponent<BottleInfo>().ourPosition)
            {
                case 1:
                    firstPlantedBottle.transform.position = new Vector3(0.1782888f, 1.789998f, -8.921255f);
                    break;
                case 2:
                    firstPlantedBottle.transform.position = new Vector3(0.513f, 1.789998f, -8.921255f);
                    break;
                case 3:
                    firstPlantedBottle.transform.position = new Vector3(0.843f, 1.789998f, -8.921255f);
                    break;
                case 4:
                    firstPlantedBottle.transform.position = new Vector3(1.135f, 1.789998f, -8.921255f);
                    break;
                    //и так далее если будет больше позиций...
            }
            firstPlantedBottle.GetComponent<BottleInfo>().position = 0;
            switch (secondPlantedBottle.GetComponent<BottleInfo>().ourPosition)
            {
                case 1:
                    secondPlantedBottle.transform.position = new Vector3(0.1782888f, 1.789998f, -8.921255f);
                    break;
                case 2:
                    secondPlantedBottle.transform.position = new Vector3(0.513f, 1.789998f, -8.921255f);
                    break;
                case 3:
                    secondPlantedBottle.transform.position = new Vector3(0.843f, 1.789998f, -8.921255f);
                    break;
                case 4:
                    secondPlantedBottle.transform.position = new Vector3(1.135f, 1.789998f, -8.921255f);
                    break;
                    //и так далее если будет больше позиций...
            }
            secondPlantedBottle.GetComponent<BottleInfo>().position = 0;
            //анимка полюбэ даже если он angry переходит в неверно и затем через hasExitTime вернется в idle
            foolAnimator.SetBool("isNotCheering", true);
            Invoke(nameof(ResetIsNotCheering), 1f);
            //дурачок ругает игрока звуком
            GameObject notCheeringSoundClone = Instantiate(notCheeringSound[Random.Range(0, notCheeringSound.Length)], startingGame.mainCamera.transform.position, Quaternion.identity);
            AudioSource audio = notCheeringSoundClone.GetComponent<AudioSource>();
            audio.Play();
            float audioLength = audio.clip.length;
            Destroy(notCheeringSoundClone, audioLength + 0.5f); //дать дурачку договорить
            GameObject bottleReturnedBackSoundClone = Instantiate(bottleReturnedBackSound, startingGame.mainCamera.transform.position, Quaternion.identity);
            AudioSource audio2 = bottleReturnedBackSoundClone.GetComponent<AudioSource>();
            audio2.Play();
            float audioLength2 = audio2.clip.length;
            Destroy(bottleReturnedBackSoundClone, audioLength2);
        }
        //счетчик заплэнченных бутылок и взятых бутылок зануливается
        controlsScript.countPlantedBottles = 0;
        controlsScript.countTakenBottles = 0;
        //флажок isNeedToReact становится обратно в false
        controlsScript.isNeedToReact = false;
        //хранимый объект в controlsScript найденной бутылки, которую выбрал пользователь зануляем
        controlsScript.correctOrNotPlantedBottle = null;
        isReacting = false;
    }
    public bool IsFoolSpeaking()
    {
        GameObject[] foolSpeechClone = GameObject.FindGameObjectsWithTag("FoolSpeech");
        return foolSpeechClone.Length > 0;
    }
    public void ResetText()
    {
        text.enabled = false;
        text.text = "";
        text.color = Color.white;
    }
    public void LaunchTheEnd()
    {
        GameObject theEndSoundClone = Instantiate(theEndSound, startingGame.mainCamera.transform.position, Quaternion.identity);
        AudioSource audio = theEndSoundClone.GetComponent<AudioSource>();
        audio.Play();
        float audioLength = audio.clip.length;
        Destroy(theEndSoundClone, audioLength + 0.5f); //дать дурачку договорить
        text.enabled = true;
        text.color = Color.yellow;
        text.text = "КОНЕЦ ИГРЫ, ТЫ ИГРАЛ УЖАСНО)))";
    }
}
