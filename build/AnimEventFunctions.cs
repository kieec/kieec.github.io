using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventFunctions : MonoBehaviour {

	public GameObject chair;
	public GameObject desk;

	public List<GameObject> targetObject;

	private GameObject chairOriginalParent;
	private Vector3 chairOriginalLocalPosition;
	private Quaternion chairOriginalLocalRotation;

	private Vector3 deskOriginalLocalPosition;
	private Quaternion deskOriginalLocalRotation;

	private Animator anim;
//	private AnimatorStateInfo infoLayer2;
//	private bool getStateInfo = false;

	// Use this for initialization
	void Start () {
		chairOriginalParent = chair.transform.parent.gameObject;
		chairOriginalLocalPosition = chair.transform.localPosition;
		chairOriginalLocalRotation = chair.transform.localRotation;

		deskOriginalLocalPosition = desk.transform.localPosition;
		deskOriginalLocalRotation = desk.transform.localRotation;

		anim = this.GetComponent<Animator> ();
	}

	// Update is called once per frame
//	void Update () {
//		if (getStateInfo) {
//			infoLayer2 = anim.GetCurrentAnimatorStateInfo(2);
//		}
//	}

	void EnableDirectly (int index){
		targetObject [index].gameObject.SetActive (true);
	}

	void DisableDirectly (int index){
		targetObject [index].gameObject.SetActive (false);
	}

//	IEnumerator SetBackLayerWeight(string stateName){
//		yield return new WaitWhile (() => infoLayer2.normalizedTime<1.0f);
//		anim.SetLayerWeight (2, 0);
//		anim.SetLayerWeight (1, 1);
//		Debug.Log ("Back");
//		getStateInfo = false;
//
//	}

	void PlayFacialAnim(string stateName){
//		getStateInfo = true;
//		anim.SetLayerWeight (2, 1f);
//		anim.SetLayerWeight (1, 0f);
//		Debug.Log ("Set");
//		Debug.Log (infoLayer2.normalizedTime);
//		anim.Play (stateName, 2, Random.Range(0f,0.5f));
		anim.Play (stateName, 1, 0.0f);
//		StartCoroutine (SetBackLayerWeight (stateName));


	}



	IEnumerator ColorFadeIn(Renderer rd, MaterialPropertyBlock pb){
		float counter = 0;
		float period = 1;
		while (counter < period)
		{
			counter += Time.deltaTime;
			//Fade from 1 to 0
			float alpha = Mathf.Lerp(0, 1, counter / period);

			//Change alpha only
//			rd.material.color = new Color(1, 1, 1, alpha);
			pb.SetColor ("_Color", new Color (1, 1, 1, alpha));
			rd.SetPropertyBlock(pb);
//			Debug.Log ("SetPropertyBlock: "+alpha);

			//Wait for a frame
			yield return null;
		}
	}

	IEnumerator ColorFadeOut(Renderer rd, MaterialPropertyBlock pb){
		float counter = 0;
		float period = 1;

		while (counter < period)
		{
			counter += Time.deltaTime;
			//Fade from 1 to 0
			float alpha = Mathf.Lerp(1, 0, counter / period);

			//Change alpha only
//			rd.material.color = new Color(1, 1, 1, alpha);
			pb.SetColor ("_Color", new Color (1, 1, 1, alpha));
			rd.SetPropertyBlock(pb);

			//Wait for a frame
			yield return null;
		}
		rd.enabled = false;
		pb.SetColor ("_Color", new Color (1, 1, 1, 1));
		rd.SetPropertyBlock(pb);
	}



	public void Show(int index){
		Renderer rd;
		rd = targetObject [index].gameObject.GetComponent<Renderer> ();

		if (rd == null) {
			foreach (Transform child in targetObject [index].transform) {
				if (child.gameObject.GetComponent<ParticleSystem> ()) {
					child.gameObject.SetActive(true);
				}
				child.gameObject.GetComponent<Renderer> ().enabled = true;
			}

		} else {
			if (rd.gameObject.GetComponent<ParticleSystem> ()) {
				rd.gameObject.SetActive(true);
			}
			rd.enabled = true;
		}

	}

	public void Hide(int index) {
		Renderer rd;
		rd = targetObject [index].gameObject.GetComponent<Renderer> ();

		if (rd == null) {
			foreach (Transform child in targetObject [index].transform) {
				child.gameObject.GetComponent<Renderer> ().enabled = false;
				if (child.gameObject.GetComponent<ParticleSystem> ()) {
					child.gameObject.SetActive(false);
				}
			}
		} else {
			rd.enabled = false;
			if (rd.gameObject.GetComponent<ParticleSystem> ()) {
				rd.gameObject.SetActive(false);
			}
		}
	}


	public void FadeInShow(int index){
		Renderer rd;
		MaterialPropertyBlock _propBlock;

		rd = targetObject [index].gameObject.GetComponent<Renderer> ();
		_propBlock = new MaterialPropertyBlock();

		if (rd == null) {
			foreach (Transform child in targetObject [index].transform) {
				
				Renderer rdChild;
				rdChild = child.gameObject.GetComponent<Renderer> ();

				rdChild.GetPropertyBlock (_propBlock);
				_propBlock.SetColor ("_Color", new Color (1, 1, 1, 0));
				rdChild.SetPropertyBlock(_propBlock);

				rdChild.enabled = true;
				StartCoroutine(ColorFadeIn (rdChild, _propBlock));
			}
		} else {
			
			rd.GetPropertyBlock (_propBlock);
			_propBlock.SetColor ("_Color", new Color (1, 1, 1, 0));
			rd.SetPropertyBlock(_propBlock);

			rd.enabled = true;
			StartCoroutine(ColorFadeIn (rd, _propBlock));
		}
	}

	public void FadeOutHide(int index) {
		Renderer rd;
		MaterialPropertyBlock _propBlock;

		rd = targetObject [index].gameObject.GetComponent<Renderer> ();
		_propBlock = new MaterialPropertyBlock();

		if (rd == null) {
			foreach (Transform child in targetObject [index].transform) {
				Renderer rdChild;
				rdChild = child.gameObject.GetComponent<Renderer> ();
				rdChild.GetPropertyBlock (_propBlock);
				_propBlock.SetColor ("_Color", new Color (1, 1, 1, 1));
				rdChild.SetPropertyBlock(_propBlock);

				StartCoroutine(ColorFadeOut (rdChild, _propBlock));
//				child.gameObject.GetComponent<Renderer> ().enabled = false; 
			}
		} else {
			rd.GetPropertyBlock (_propBlock);
			_propBlock.SetColor ("_Color", new Color (1, 1, 1, 1));
			rd.SetPropertyBlock(_propBlock);
			StartCoroutine(ColorFadeOut (rd, _propBlock));
//			rd.enabled = false;
		}
	}

	public void PlayAnim(int index) {

		targetObject[index].gameObject.GetComponent<Animation>().Play(); 

	}

	public void StopAnim(int index) {

		targetObject[index].gameObject.GetComponent<Animation>().Stop(); 

	}

	public void RewindAnim(int index) {
		Animation anim;
		anim = targetObject [index].gameObject.GetComponent<Animation> ();

		foreach (AnimationState state in anim)
		{
			state.speed = 1.0f;
			state.time = 0;
		}

	}

	public void PlayAnimBackward(int index) {
		Animation anim;
		anim = targetObject [index].gameObject.GetComponent<Animation> ();
		foreach (AnimationState state in anim)
		{
			state.speed = -1.0f;
			state.time = anim.clip.length;
		}
		anim.Play(); 

	}

	public void ParentChairToAnimObj(int index) {
		chair.transform.SetParent (targetObject [index].transform);

	}

	public void UnparentChair() {
		chair.transform.SetParent (chairOriginalParent.transform);
		chair.transform.localPosition = chairOriginalLocalPosition;
		chair.transform.localRotation = chairOriginalLocalRotation;
	}

	public void ParentDeskToAnimObj(int index) {
		desk.transform.SetParent (targetObject [index].transform);

	}

	public void UnparentDesk() {
		desk.transform.SetParent (chairOriginalParent.transform);
		desk.transform.localPosition = deskOriginalLocalPosition;
		desk.transform.localRotation = deskOriginalLocalRotation;
	}





}
