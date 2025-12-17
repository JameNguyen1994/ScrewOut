using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using System.Collections.Generic;
using RSG;
using UnityEngine.Networking;

public class MainScript : MonoBehaviour {

	private readonly string basePath = "https://jsonplaceholder.typicode.com";
	private RequestHelper currentRequest;

	private void LogMessage(string title, string message) {
#if UNITY_EDITOR
		EditorUtility.DisplayDialog (title, message, "Ok");
#else
		Debug.Log(message);
#endif
	}

	private PromiseTimer timer;
	
	private void Start()
	{
		timer = new PromiseTimer();
		StartCoroutine(Test());
	}

	private void Update()
	{
		timer.Update(Time.deltaTime);
	}

	IEnumerator Test()
	{
		var waiter = GetStringWait4S();
		yield return waiter;
		var waiter2 = waiter.Then(str =>
		{
			Debug.Log(str);
			print(str);
			return GetStringWait4S();
		});

		yield return waiter2;

		waiter2.Then(print);
	}


	private IPromise<string> GetStringWait4S()
	{
		print("Vao de call");
		var promise = new Promise<string>();
		timer.WaitFor(4f).Then(() =>
		{
			print("vao sau 4s ne");
			promise.Resolve("may biet tao sao?");
		});

		return promise;
	}

	public void Get(){

		// We can add default request headers for all requests
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";

		RequestHelper requestOptions = null;

		RestClient.GetArray<Post>(basePath + "/posts").Then(res => {
			this.LogMessage("Posts", JsonHelper.ArrayToJsonString<Post>(res, true));
			return RestClient.GetArray<Todo>(basePath + "/todos");
		}).Then(res => {
			this.LogMessage("Todos", JsonHelper.ArrayToJsonString<Todo>(res, true));
			return RestClient.GetArray<User>(basePath + "/users");
		}).Then(res => {
			this.LogMessage("Users", JsonHelper.ArrayToJsonString<User>(res, true));

			// We can add specific options and override default headers for a request
			requestOptions = new RequestHelper { 
				Uri = basePath + "/photos",
				Headers = new Dictionary<string, string> {
					{ "Authorization", "Other token..." }
				},
				EnableDebug = true
			};
			return RestClient.GetArray<Photo>(requestOptions);
		}).Then(res => {
			this.LogMessage("Header", requestOptions.GetHeader("Authorization"));

			// And later we can clear the default headers for all requests
			RestClient.ClearDefaultHeaders();

		}).Catch(err => this.LogMessage("Error", err.Message));
	}

	IEnumerator Check()
	{
		RestClient.DefaultRequestParams["param1"] = "My first param";
		RestClient.DefaultRequestParams["param3"] = "My other param";

		currentRequest = new RequestHelper {
			Uri = basePath + "/posts",
			Params = new Dictionary<string, string> {
				{ "param1", "value 1" },
				{ "param2", "value 2" }
			},
			Body = new Post {
				title = "foo",
				body = "bar",
				userId = 1
			},
			EnableDebug = true
		};

		var waiter = RestClient.Post<Post>(currentRequest);
		
		yield return waiter;
		
		waiter.Then(res =>
		{
			print(res.body);
		});
	}
	
	public void Post(){

		// We can add default query string params for all requests
		RestClient.DefaultRequestParams["param1"] = "My first param";
		RestClient.DefaultRequestParams["param3"] = "My other param";

		currentRequest = new RequestHelper {
			Uri = basePath + "/posts",
			Params = new Dictionary<string, string> {
				{ "param1", "value 1" },
				{ "param2", "value 2" }
			},
			Body = new Post {
				title = "foo",
				body = "bar",
				userId = 1
			},
			EnableDebug = true
		};
		RestClient.Post<Post>(currentRequest)
		.Then(res => {

			// And later we can clear the default query string params for all requests
			RestClient.ClearDefaultParams();

			this.LogMessage("Success", JsonUtility.ToJson(res, true));
		})
		.Catch(err => this.LogMessage("Error", err.Message));
	}

	public void Put(){

		currentRequest = new RequestHelper {
			Uri = basePath + "/posts/1", 
			Body = new Post {
				title = "foo",
				body = "bar",
				userId = 1
			},
			Retries = 5,
			RetrySecondsDelay = 1,
			RetryCallback = (err, retries) => {
				Debug.Log (string.Format ("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
			}
		};
		RestClient.Put<Post>(currentRequest, (err, res, body) => {
			if (err != null){
				this.LogMessage("Error", err.Message);
			}
			else {
				this.LogMessage("Success", JsonUtility.ToJson(body, true));
			}
		});
	}

	public void Delete(){

		RestClient.Delete(basePath + "/posts/1", (err, res) => {
			if (err != null){
				this.LogMessage("Error", err.Message);
			}
			else {
				this.LogMessage("Success", "Status: " + res.StatusCode.ToString());
			}
		});
	}

	public void AbortRequest(){
		if (currentRequest != null) {
			currentRequest.Abort();
			currentRequest = null;
		}
	}

	public void DownloadFile(){

		//RestClient.ClearDefaultParams();
		
		print("call download file");
		
		var fileUrl = "https://www.dropbox.com/scl/fi/e471ud0d6qud5sz798h9j/musicgame_2.mp3?rlkey=29vjnsq6lp97ej91tj2deiqt7&st=spwkrvt2&dl=1";
		var fileType = AudioType.MPEG;

		RestClient.Get(new RequestHelper {
			Uri = fileUrl,
			DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType),
			EnableDebug = true,
			ProgressCallback = (progress) => {Debug.Log($"progress: {(int)(progress * 100)}%");}
		}).Progress((progress) =>
		{
			Debug.Log($"progress: {(int)(progress * 100)}%");
		}).Then(res => {
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = ((DownloadHandlerAudioClip)res.Request.downloadHandler).audioClip;
			audio.Play();
		}).Catch(err => {
			this.LogMessage("Error", err.Message);
		});
	}
}