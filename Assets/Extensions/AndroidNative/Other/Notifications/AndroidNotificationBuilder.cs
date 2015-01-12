using UnityEngine;
using System.Collections;

public class AndroidNotificationBuilder {
	private int _id;
	private string _title;
	private string _message;
	private int _time;
	private string _sound;
	private string _icon;
	private bool _vibration;

	private const string SOUND_SILENT = "SOUND_SILENT";

	public AndroidNotificationBuilder(int id, string title, string message, int time) {
		_id = id;
		_title = title;
		_message = message;
		_time = time;

		_icon = AndroidNativeSettings.Instance.LocalNotificationIcon == null ? string.Empty : AndroidNativeSettings.Instance.LocalNotificationIcon.name;
		_sound = AndroidNativeSettings.Instance.LocalNotificationSound == null ? string.Empty : AndroidNativeSettings.Instance.LocalNotificationSound.name;
		_vibration = AndroidNativeSettings.Instance.EnableVibrationLocal;
	}

	public void SetSoundName(string sound) {
		_sound = sound;
	}

	public void SetIconName(string icon) {
		_icon = icon;
	}

	public void SetVibration(bool vibration) {
		_vibration = vibration;
	}

	public void SetSilentNotification() {
		_sound = SOUND_SILENT;
	}

	public int Id {
		get {
			return _id;
		}
	}

	public string Title {
		get {
			return _title;
		}
	}

	public string Message {
		get {
			return _message;
		}
	}

	public int Time {
		get {
			return _time;
		}
	}

	public string Sound {
		get {
			return _sound;
		}
	}

	public string Icon {
		get {
			return _icon;
		}
	}

	public bool Vibration {
		get {
			return _vibration;
		}
	}
}
