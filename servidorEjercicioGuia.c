#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>

int contador;

// Evita errores de threads
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

void *AtenderCliente(void *socket)
{
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn = *s;

	char peticion[512];
	char respuesta[512];

	int ret;

	// Bucle de atencion al cliente
		int terminar = 0;
		while (terminar == 0)
		{
			// Ahora recibimos su peticion
			ret=read(sock_conn,peticion, sizeof(peticion));
			printf ("Recibida una peticion\n");
			// Tenemos que a?adirle la marca de fin de string 
			// para que no escriba lo que hay despues en el buffer
			peticion[ret]='\0';
		
			//Escribimos la peticion en la consola
		
			printf ("La peticion es: %s\n",peticion);
			char *p = strtok(peticion, "/");
			int codigo =  atoi (p);
			char nombre[20];

			if(codigo != 0 && codigo != 4){
				p = strtok( NULL, "/");
				strcpy (nombre, p);
				printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
			}
			else printf ("Codigo: %d\n", codigo);

			if (codigo == 0) terminar = 1;
			else if (codigo == 4) 
				sprintf (respuesta,"%d", contador);
			else if (codigo == 1) //piden la longitd del nombre
				sprintf (respuesta,"%d",strlen (nombre));
			else if (codigo == 2)
				// quieren saber si el nombre es bonito
				if((nombre[0]=='M') || (nombre[0]=='S'))
				strcpy (respuesta,"SI");
				else
					strcpy (respuesta,"NO");
				
			else // Decir si es alto
			{
				p = strtok(NULL, "/");
				float altura = atof(p);
				if (altura > 1.7)
				{
					sprintf (respuesta, "%s : eres alto", nombre);
				}
				else
				{
					sprintf(respuesta, "%s : eres un tapón", nombre);
				}
			}
			printf("Respuesta: %s\n", respuesta);
			// Enviamos la respuesta (si hay)
			if (codigo != 0) write (sock_conn,respuesta, strlen(respuesta));

			if (codigo == 1 || codigo == 2 || codigo == 3)
			{
				pthread_mutex_lock(&mutex); // No interrumpir a partir de ahora
				contador++; // Incrementamos el contador de servicios
				pthread_mutex_unlock(&mutex); // Ya se puede interrumpir
			} 
		}
		// Se acabo el servicio para este cliente
		close(sock_conn);	
		printf("Check");
}
int main(int argc, char *argv[])
{
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");

	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	// asocia el socket a cualquiera de las IP de la maquina.

	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);

	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9060);

	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");

	//La cola de peticiones pendientes no podra ser superior a 4
	if (listen(sock_listen, 4) < 0)
		printf("Error en el Listen\n");

	contador = 0;
	int i;
	int sockets[100];
	pthread_t thread;

	for(;;){ // Bucle infinito -> infinitas peticiones
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		
		sockets[i] = sock_conn;
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		
	}
}
