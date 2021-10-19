using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneOnDisconnect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(WaitForDC());
    }

    IEnumerator WaitForDC()
    {
        yield return new WaitUntil(() => SslClient.sslStream != null);

        yield return new WaitForSeconds(3);

        while (true) {
            yield return new WaitForEndOfFrame();

            bool shouldReload = CheckIfShouldReload();
            bool CheckIfShouldReload()
            {
                if (SslClient.sslStream == null) {
                    Debug.Log("stream is null");
                    return true;
                }

                if (SslClient.sslStream.IsClosed) {
                    Debug.Log("Stream is closed");
                    return true;
                }

                return false;
            }
            

            if (shouldReload) {
                SceneManager.LoadScene(0);
            }
        }
    }
}
