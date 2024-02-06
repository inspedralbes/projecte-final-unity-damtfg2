using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



namespace StarterAssets
{
    public class Teleport : NetworkBehaviour
    {

        public GameObject marca;
        public static List<Marca> marks = new List<Marca>();
        public static List<MarcaObject> markObjects = new List<MarcaObject>();
        public GameObject nuevaMarca;
        public GameObject currentMark;


        public void CrearMarca()
        {
            // Obt�n la posici�n del jugador
            Vector3 posicionJugador = transform.position;
            // Crea la "Marca" en la posici�n del 
            if (HasMarks())
            {
                BorrarMarca();
            }
            SpawnMarkServerRpc(posicionJugador);

            // Guarda la posici�n de la marca en la variable
        }

        public void BorrarMarca()
        {
            Marca? mark = SearchForCreatorMark();
            if (mark != null)
            {
                DespawnMarkServerRpc(mark.Value);
                RemoveMarkClientRpc(mark.Value);
            }
        }
        public void BorrarMarca2()
        {
            Marca? mark = SearchForOwnerMark2();
            if (mark != null)
            {
                DespawnMarkServerRpc(mark.Value);
                RemoveMarkClientRpc(mark.Value);
            }
        }

        public void TakeMark(Marca mark)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                if (marks[i].Equals(mark))
                {
                    ChangeMarkObjectRpc(new Marca(marks[i].posicion, marks[i].JugadorCreador, NetworkManager.LocalClientId), marks[i]);
                    ChangeMarkClientRpc(new Marca(marks[i].posicion, marks[i].JugadorCreador, NetworkManager.LocalClientId), marks[i]);
                }
            }
        }

        public Marca? SearchForCreatorMark()
        {
            foreach (Marca marca in marks)
            {
                if (marca.JugadorCreador == NetworkManager.LocalClientId)
                {
                    return marca;
                }
            }
            return null;
        }
    
        public Marca? SearchForOwnerMark()
        {
            foreach (Marca marca in marks)
            {
                if (marca.JugadorAsociado == NetworkManager.LocalClientId && marca.JugadorCreador == NetworkManager.LocalClientId)
                {
                    return marca;

                }
            }
            return null;
        }
        public Marca? SearchForOwnerMark2()
        {
            foreach (Marca marca in marks)
            {
                if (marca.JugadorAsociado == NetworkManager.LocalClientId && marca.JugadorCreador != NetworkManager.LocalClientId)
                {
                    return marca;

                }
            }
            return null;
        }

        public bool HasMarks()
        {            
            return SearchForCreatorMark() != null ? true : false;
        }

        [Rpc(SendTo.Server)]
        public void SpawnMarkServerRpc(Vector3 posicionJugador, RpcParams rpcParams = default)
        {
            // Crea la "Marca" en la posici�n del jugador
            
            nuevaMarca = Instantiate(marca, posicionJugador, Quaternion.identity);
            nuevaMarca.GetComponent<NetworkObject>().Spawn(true);
            //posicionDeLaMarca = nuevaMarca.transform.position;
            Marca mark = new Marca(nuevaMarca.transform.position, rpcParams.Receive.SenderClientId, rpcParams.Receive.SenderClientId);
            markObjects.Add(new MarcaObject(nuevaMarca, mark));
            AddNewMarkClientRpc(mark);
        }

        [Rpc(SendTo.Everyone)]
        public void AddNewMarkClientRpc(Marca mark)
        {
            marks.Add(mark);
        }

        [Rpc(SendTo.Everyone)]
        public void ChangeMarkClientRpc(Marca NewMark, Marca OldMark)
        {
            int index = marks.FindIndex(x => x.Equals(OldMark));
            marks[index] = NewMark;
        }
        [Rpc(SendTo.Server)]
        public void ChangeMarkObjectRpc(Marca NewMark, Marca OldMark)
        {
            int index = markObjects.FindIndex(x => x.marca.Equals(OldMark));
            markObjects[index].marca = NewMark;
        }
        [Rpc(SendTo.Everyone)]
        public void RemoveMarkClientRpc(Marca mark)
        {
            marks.Remove(mark);
        }

        [Rpc(SendTo.Server)]
        public void DespawnMarkServerRpc(Marca mark, RpcParams serverRpcParams = default)
        {
            MarcaObject marktoremove = null;
            foreach (MarcaObject marcaObject in markObjects)
            {
                if (marcaObject.marca.Equals(mark))
                {
                    marktoremove = marcaObject;
                    marcaObject.objeto.GetComponent<NetworkObject>().Despawn(true);
                }
            }
            if (marktoremove != null) { 

                markObjects.Remove(marktoremove);
            }
            // Llama a CambiarMarca despu�s de un peque�o retraso (0.1 segundos)

        }

        public Vector3 GivePos()
        {

            Marca? mark = SearchForOwnerMark();
            if (mark != null)
            {
                return mark.Value.posicion;
            }
            else
            {
                return Vector3.zero;
            }
        }
        public Vector3 GivePos2()
        {

            Marca? mark = SearchForOwnerMark2();
            if (mark != null)
            {
                return mark.Value.posicion;
            }
            else
            {
                return Vector3.zero;
            }
        }
        public Marca GetMarca(Vector3 gameObject)
        {
            foreach(Marca marca in marks)
            {
                if (marca.posicion == (gameObject))
                {
                    return marca;
                }
            }
            Debug.Log("No encontre");
            return new Marca(Vector3.zero, unchecked((ulong)-1), unchecked((ulong)-1));
        }
    }

}
public class MarcaObject
{
    public GameObject objeto;
    public Marca marca;
    public MarcaObject(GameObject obj, Marca mark)
    {
        objeto = obj;
        marca = mark;
    }
}