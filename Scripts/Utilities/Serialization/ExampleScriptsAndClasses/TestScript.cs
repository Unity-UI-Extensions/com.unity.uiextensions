using UnityEngine;
using UnityEngine.UI.Extensions;

public class TestScript : MonoBehaviour {

	public string testString = "Hello";
	public GameObject someGameObject;
	public string someGameObject_id;
	public TestClass testClass = new TestClass();
	public TestClass[] testClassArray = new TestClass[2];
	[DontSaveField] public Transform TransformThatWontBeSaved;//The [DontSaveField] attribute we wrote ourselves prevents the field from being included in the packed component data

	public void OnSerialize() {
		//This is an example of a OnSerialize method, called before a gameobject is packed into serializable form.
		//In this case, the GameObject variable "someGameObject" and those in the testClass and testclass Array instances of TestClass should be reconstructed after loading.
		//Since GameObject (and Transform) references assigned during runtime can't be serialized directly, 
		//we keep a seperate string variable for each GO variable that holds the ID of the GO instead.
		//This allows us to just save the ID instead.

		//This example is one way of dealing with GameObject (and Transform) references. If a lot of those occur in your project,
		//it might be more efficient to go directly into the static SaveLoad.PackComponent method. and doing it there.

		if(someGameObject != null && someGameObject.GetComponent<ObjectIdentifier>()) {
			someGameObject_id = someGameObject.GetComponent<ObjectIdentifier>().id;
		}
		else {
			someGameObject_id = null;
		}

		if(testClassArray != null) {
			foreach(TestClass testClass_cur in testClassArray) {
				if(testClass_cur.go != null && testClass_cur.go.GetComponent<ObjectIdentifier>()) {
					testClass_cur.go_id = testClass_cur.go.GetComponent<ObjectIdentifier>().id;
				}
				else {
					testClass_cur.go_id = null;
				}
			}

		}
	}

	public void OnDeserialize() {

		//Since we saved the ID of the GameObject references, we can now use those to recreate the references. 
		//We just iterate through all the ObjectIdentifier component occurences in the scene, compare their id value to our saved and loaded someGameObject id (etc.) value,
		//and assign the component's GameObject if it matches.
		//Note that the "break" command is important, both because it elimitates unneccessary iterations, 
		//and because continuing after having found a match might for some reason find another, wrong match that makes a null reference.

		ObjectIdentifier[] objectsIdentifiers = FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[];

		if(string.IsNullOrEmpty(someGameObject_id) == false) {
			foreach(ObjectIdentifier objectIdentifier in objectsIdentifiers) {

				if(string.IsNullOrEmpty(objectIdentifier.id) == false) {
					if(objectIdentifier.id == someGameObject_id) {
						someGameObject = objectIdentifier.gameObject;
						break;
					}
				}
			}
		}

		if(testClassArray != null) {
			foreach(TestClass testClass_cur in testClassArray) {
				if(string.IsNullOrEmpty(testClass_cur.go_id) == false) {
					foreach (ObjectIdentifier objectIdentifier in objectsIdentifiers) {
						if(string.IsNullOrEmpty(objectIdentifier.id) == false) {
							if(objectIdentifier.id == testClass_cur.go_id) {
								testClass_cur.go = objectIdentifier.gameObject;
								break;
							}
						}
					}
				}
			}
		}
	}
}

