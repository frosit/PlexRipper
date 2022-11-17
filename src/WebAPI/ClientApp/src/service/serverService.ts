import { Context } from '@nuxt/types';
import { Observable, of } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';
import { PlexServerDTO, PlexServerStatusDTO } from '@dto/mainApi';
import { checkPlexServer, getPlexServers } from '@api/plexServerApi';
import IStoreState from '@interfaces/service/IStoreState';
import { BaseService } from '@service';

export class ServerService extends BaseService {
	// region Constructor and Setup
	public constructor() {
		super('ServerService', {
			// Note: Each service file can only have "unique" state slices which are not also used in other service files
			stateSliceSelector: (state: IStoreState) => {
				return {
					servers: state.servers,
				};
			},
		});
	}

	public setup(nuxtContext: Context): Observable<any> {
		super.setNuxtContext(nuxtContext);
		return this.refreshPlexServers().pipe(take(1));
	}

	// endregion

	// region Fetch

	public fetchServers(): void {
		this.refreshPlexServers().subscribe();
	}

	public refreshPlexServers(): Observable<PlexServerDTO[]> {
		return getPlexServers().pipe(
			tap((plexServers) => {
				if (plexServers.isSuccess) {
					this.setStoreProperty(
						'servers',
						plexServers.value,
						'ServerService => refreshPlexServers: Servers cache updated',
					);
				}
			}),
			switchMap(() => this.getServers()),
		);
	}

	// endregion

	public get servers(): PlexServerDTO[] {
		return this.getState().servers;
	}

	public getServers(ids: number[] = []): Observable<PlexServerDTO[]> {
		return this.stateChanged.pipe(
			map((state: IStoreState) => state?.servers ?? []),
			map((serverList) => serverList.filter((server) => !ids.length || ids.includes(server.id))),
		);
	}

	public getServer(serverId: number): Observable<PlexServerDTO | null> {
		return this.getServers().pipe(
			switchMap((servers: PlexServerDTO[]) => of(servers.find((x) => x.id === serverId) ?? null)),
		);
	}

	public checkServer(plexServerId: number): Observable<PlexServerStatusDTO | null> {
		return checkPlexServer(plexServerId).pipe(
			map((serverStatus) => {
				if (serverStatus.isSuccess && serverStatus.value) {
					const servers = this.getState().servers;
					const index = servers.findIndex((x) => x.id === serverStatus.value?.plexServerId);
					if (index === -1) {
						return serverStatus?.value ?? null;
					}
					const server = servers[index];
					server.status = serverStatus.value;
					servers.splice(index, 1, server);
					this.setState({ servers }, 'Update server status for ' + plexServerId);
				}
				return serverStatus?.value ?? null;
			}),
		);
	}
}

const serverService = new ServerService();
export default serverService;
