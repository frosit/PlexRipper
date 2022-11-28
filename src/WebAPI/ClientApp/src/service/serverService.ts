import { Context } from '@nuxt/types';
import { combineLatest, Observable, of } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';
import { PlexAccountDTO, PlexServerDTO, PlexServerStatusDTO } from '@dto/mainApi';
import { checkPlexServer, getPlexServers } from '@api/plexServerApi';
import IStoreState from '@interfaces/service/IStoreState';
import { AccountService, BaseService } from '@service';
import ISetupResult from '@interfaces/service/ISetupResult';

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

	public setup(nuxtContext: Context): Observable<ISetupResult> {
		super.setup(nuxtContext);
		return this.refreshPlexServers().pipe(
			switchMap(() => of({ name: this._name, isSuccess: true })),
			take(1),
		);
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
					this.setStoreProperty('servers', plexServers.value);
				}
			}),
			map(() => this.getStoreSlice<PlexServerDTO[]>('servers')),
			take(1),
		);
	}

	// endregion

	public getServers(ids: number[] = []): Observable<PlexServerDTO[]> {
		return this.stateChanged.pipe(
			map((state: IStoreState) => state.servers ?? []),
			map((serverList) => serverList.filter((server) => !ids.length || ids.includes(server.id))),
		);
	}

	public getServer(serverId: number): Observable<PlexServerDTO | null> {
		return this.getServers([serverId]).pipe(map((servers) => (servers.length > 0 ? servers[0] : null)));
	}

	/**
	 * Retrieves the accessible PlexServer for the given PlexAccount
	 * @param {Number} plexAccountId
	 */
	public getServersByPlexAccountId(plexAccountId: number): Observable<PlexServerDTO[]> {
		return combineLatest([this.getStateChanged<PlexServerDTO[]>('servers'), AccountService.getAccount(plexAccountId)]).pipe(
			map(([servers, account]: [PlexServerDTO[], PlexAccountDTO | null]) => {
				if (!account) {
					return [];
				}
				const serverIds = account.plexServerAccess.map((x) => x.plexServerId);
				return servers.filter((server) => serverIds.includes(server.id));
			}),
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
