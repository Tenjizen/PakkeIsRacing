using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;

 //   private void Start()
 //   {
	//	transform.localScale = new Vector3(0, 0, 0);
	//	StartCoroutine("Show");
	//}
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
						this.gameObject.SetActive(false);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}

	//IEnumerator Show()
 //   {
	//	Debug.Log("Show");
	//	yield return new WaitForSeconds(5f);
	//	//transform.localScale = new Vector3(5, 5, 5);
	//	Debug.Log("apres show");
	//	StartCoroutine("Fade");
	//	Debug.Log("lancer fade");
	//}

	//IEnumerator Fade()
 //   {
	//	Debug.Log("fade lancé");
	//	while(transform.localScale != new Vector3(5, 5, 5))
 //       {
	//		transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
	//		yield return new WaitForSeconds(0.2f);
	//		Debug.Log("fade en cours");
	//	}
	//}
}
