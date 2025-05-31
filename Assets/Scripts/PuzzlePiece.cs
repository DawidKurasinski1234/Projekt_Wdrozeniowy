using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class PuzzlePiece: MonoBehaviour {
    private int m_x, m_y;
    private GameObject m_manager;
    
    private void
    Start()
    {
        m_manager = GameObject.Find("_PuzzleManager");
        Match match = Regex.Match(name, "Piece (\\d+)x(\\d+)");
        if (!match.Success)
            throw new InvalidDataException("Invalid PuzzlePiece name");
        m_x = int.Parse(match.Groups[1].Value);
        m_y = int.Parse(match.Groups[2].Value);
    }
    
    private void
    OnMouseDown()
    {
        m_manager.GetComponent<PuzzleManager>().OnPieceSelected(m_x, m_y);
    }
}
