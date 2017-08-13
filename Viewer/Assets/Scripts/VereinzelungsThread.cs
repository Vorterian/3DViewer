
using System.IO;

public class VereinzelungsThread
{

    private int[] _listeAllerDreiecksPunkte;
    private int _startpunkt;
    private int _endpunkt;
    private int _gesuchterErsterDreiecksPunkt;
    private int _gesuchterZweiterDreiecksPunkt;
    private int _gesuchterDritterDreiecksPunkt;
    private ModellViewer _mV;
    private bool _shouldStop = false;
    private StreamWriter _logger;


    public VereinzelungsThread(int startpunkt, int endpunkt, int stuetzpunkt1, int stuetzpunkt2, int stuetzpunkt3, int[] datenAblage, ModellViewer mV, StreamWriter logger)
    {
        
        _startpunkt = startpunkt;
        _endpunkt = endpunkt;
        _gesuchterErsterDreiecksPunkt = stuetzpunkt1;
        _gesuchterZweiterDreiecksPunkt = stuetzpunkt2;
        _gesuchterDritterDreiecksPunkt = stuetzpunkt3;

        _listeAllerDreiecksPunkte = datenAblage;
        _mV = mV;
        _logger = logger;

    }


	public void read()
    {
        
        for (int indexDesDreieckStartpunktes = _startpunkt; indexDesDreieckStartpunktes <= _endpunkt; indexDesDreieckStartpunktes = indexDesDreieckStartpunktes + 3)
        {

            var gefundenderErsterDreiecksPunkt = _listeAllerDreiecksPunkte[indexDesDreieckStartpunktes];
            var gefundenderZweiterDreiecksPunkt = _listeAllerDreiecksPunkte[indexDesDreieckStartpunktes + 1];
            var gefundenderDritterDreiecksPunkt = _listeAllerDreiecksPunkte[indexDesDreieckStartpunktes + 2];


            if (_gesuchterErsterDreiecksPunkt == gefundenderErsterDreiecksPunkt && _gesuchterZweiterDreiecksPunkt == gefundenderZweiterDreiecksPunkt && _gesuchterDritterDreiecksPunkt == gefundenderDritterDreiecksPunkt)
            {
                _mV.setEinmaligesDreieck();

                break;
            }

            if (_mV.readEinmaligesDreieck() == false)
            {
                break;
            }


        }

  

    }

    public void RequestStop()
    {
        _shouldStop = true;
    }



}
