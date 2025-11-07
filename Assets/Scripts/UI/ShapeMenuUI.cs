using UnityEngine;

public class ShapeMenuUI : MonoBehaviour
{
    [Header("References")]
    public GameObject menuPanel;
    public ShapeManager shapeManager;

    private bool isMenuOpen = false;

    void Start()
    {
        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);

        if (shapeManager != null)
        {
            // Show UI shape only when menu is open
            if (shapeManager.UIShapeController != null)
                shapeManager.UIShapeController.gameObject.SetActive(isMenuOpen);

            // Show world shape only when menu is closed
            shapeManager.SetWorldShapeVisible(!isMenuOpen);
        }
    }

    public void OnGeneratePyramid() => shapeManager.GeneratePyramid();
    public void OnGenerateCylinder() => shapeManager.GenerateCylinder();
    public void OnGenerateRectangularColumn() => shapeManager.GenerateRectangularColumn();
    public void OnGenerateSphere() => shapeManager.GenerateSphere();
    public void OnGenerateCapsule() => shapeManager.GenerateCapsule();

    public void OnErase() => shapeManager.EraseShapes();
}
