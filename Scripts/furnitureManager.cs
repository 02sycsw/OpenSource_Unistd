using UnityEngine;
using UnityEngine.EventSystems;

public class furnitureManager : MonoBehaviour
{
    private GameObject selectedObject; //더블 클릭으로 선택된 오브젝트
    private Vector3 screenPoint;
    private Vector3 offset;

    private float lastClickTime = 0f; //마지막 클릭 시간
    private float doubleClickTime = 0.3f; //더블 클릭 간의 최대 시간 (초)
    private bool isDragging = false; //물체를 드래그할지 여부

    private bool isRotating = false; //회전 중인지 여부
    private float rotationSpeed = 10f; //회전 속도

    void Update()
    {
        //마우스 왼쪽 버튼 클릭 시
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) //ray가 3D 오브젝트와 충돌
            {
                GameObject clickedObject = hit.transform.gameObject; //클릭된 오브젝트

                //더블 클릭 감지
                if (Time.time - lastClickTime <= doubleClickTime)
                {
                    //더블 클릭이 감지되면 물체를 선택하고 이동 가능 상태로 설정
                    selectedObject = clickedObject;
                    isDragging = true; //드래그 시작

                    //마우스 위치와 물체 위치 사이 계산
                    screenPoint = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                    offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
                else
                {
                    //selectedObject = clickedObject;
                }

                //마지막 클릭 시간 갱신
                lastClickTime = Time.time;
            }
        }

        //물체 이동(x, z로만 이동)
        if (isDragging && selectedObject != null && Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z); //마우스의 현재 위치
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset; //월드 좌표로 변환

            curPosition.y = selectedObject.transform.position.y; // 현재 y값을 그대로 유지

            if (!IsColliding(curPosition)) //다른 물체와 충돌하지 않았을 때만 이동
                selectedObject.transform.position = curPosition;
        }

        //물체 회전
        if (selectedObject != null)
        {
            //r 키를 눌러서 90도 회전
            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedObject.transform.Rotate(Vector3.up, 90f);
                //selectedObject.transform.rotation = Quaternion.Euler(selectedObject.transform.rotation.eulerAngles.x, selectedObject.transform.rotation.eulerAngles.y + 90f, selectedObject.transform.rotation.eulerAngles.z);
            }

            //r 키를 꾹 누르면 마우스 이동에 따라 회전
            if (Input.GetKey(KeyCode.R))
            {
                isDragging = false; //회전 중 마우스 방향에 따라 물체가 이동하는 것을 방지
                isRotating = true;
                float mouseDelta = Input.GetAxis("Mouse X"); //마우스 x 이동량을 기반으로 회전
                selectedObject.transform.Rotate(Vector3.up, mouseDelta * rotationSpeed);
            }
            else
            {
                isDragging = true;
                isRotating = false;
            }
        }

        //마우스를 놓았을 때 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            selectedObject = null;
        }
    }

    //물체 충돌 검사
    private bool IsColliding(Vector3 targetPosition)
    {
        //선택된 물체의 콜라이더와 해당 범위
        Collider selectedCollider = selectedObject.GetComponent<Collider>();
        Bounds selectedBounds = selectedCollider.bounds;

        //기본 충돌 범위와 확장된 충돌 범위 설정
        Vector3 normalExtents = selectedBounds.extents;

        //기본 충돌 범위를 사용하여 벽과의 충돌 확인
        Collider[] wallColliders = Physics.OverlapBox(targetPosition, normalExtents, Quaternion.identity);
        foreach (Collider wallCollider in wallColliders)
        {
            //벽과 충돌한 경우, 확장된 범위 없이 충돌 처리
            if (wallCollider.gameObject.tag == "WALL" && wallCollider.gameObject != selectedObject)
            {
                Debug.Log("Colliding with WALL (no expansion)");
                return true;
            }
        }

        Vector3 extendedExtents = selectedBounds.extents + new Vector3(0.05f, 0.05f, 0.05f); // 5cm 확장
        //벽이 아닌 다른 객체와의 충돌 확인 (확장된 범위 사용)
        Collider[] colliders = Physics.OverlapBox(targetPosition, extendedExtents, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            //자기 자신과 "FLOOR" 태그는 제외
            if (collider.gameObject != selectedObject && collider.gameObject.tag != "FLOOR")
            {
                return true;
            }
        }

        //충돌 없음
        return false;
    }
}
