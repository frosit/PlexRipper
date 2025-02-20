<template>
	<template v-if="serverStore.getVisibleServers.length > 0">
		<q-expansion-item
			v-for="(server, index) in serverStore.getVisibleServers"
			:key="index"
			:label="serverStore.getServerName(server.id)"
			expand-icon="mdi-chevron-down">
			<!-- Server header	-->
			<template #header>
				<q-item-section
					side
					no-wrap>
					<QStatus :value="serverConnectionStore.isServerConnected(server.id)" />
				</q-item-section>

				<q-item-section>
					<div class="server-name">
						<q-icon
							v-if="server.owned"
							name="mdi-home"
							size="24px"
							left />
						{{ serverStore.getServerName(server.id) }}
					</div>
				</q-item-section>
				<q-item-section side>
					<q-btn
						icon="mdi-cog"
						flat
						:data-cy="`server-dialog-${index}`"
						@click.stop="dialogStore.openServerSettingsDialog(server.id)" />
				</q-item-section>
			</template>
			<!-- Render libraries -->
			<q-list v-if="filterLibraries(server.id).length > 0">
				<q-item
					v-for="(library, y) in filterLibraries(server.id)"
					:key="y"
					v-ripple
					clickable
					active-class="text-orange"
					@click="openMediaPage(library)">
					<q-item-section avatar>
						<QMediaTypeIcon
							:active="library.syncedAt != null"
							:loading="isLibrarySyncing(library.id)"
							:media-type="library.type" />
					</q-item-section>
					<q-item-section>{{ libraryStore.getLibraryName(library.id) }}</q-item-section>
				</q-item>
			</q-list>
			<!-- No libraries available -->
			<template v-else>
				<q-item>
					<q-item-section>{{ t('components.server-drawer.no-libraries') }}</q-item-section>
				</q-item>
			</template>
		</q-expansion-item>

		<ServerDialog />
	</template>
	<!-- With valid server available -->

	<!-- No servers available -->
	<template v-else>
		<q-item>
			<q-item-section>{{ t('components.server-drawer.no-servers.header') }}</q-item-section>
		</q-item>
		<q-item>
			<q-item-section>{{ t('components.server-drawer.no-servers.description') }}</q-item-section>
		</q-item>
	</template>
</template>

<script setup lang="ts">
import Log from 'consola';
import { type LibraryProgress, type PlexLibraryDTO, PlexMediaType } from '@dto';
import { useSubscription } from '@vueuse/rxjs';
import { get, set } from '@vueuse/core';
import {
	useLibraryStore,
	useServerStore,
	useSignalrStore,
	useDialogStore,
	useServerConnectionStore,
	useI18n,
} from '#imports';

const { t } = useI18n();
const router = useRouter();
const serverStore = useServerStore();
const libraryStore = useLibraryStore();
const dialogStore = useDialogStore();
const serverConnectionStore = useServerConnectionStore();
const signalRStore = useSignalrStore();

const libraryProgress = ref<LibraryProgress[]>([]);

function filterLibraries(plexServerId: number): PlexLibraryDTO[] {
	return libraryStore.getLibraries.filter((x) => x.plexServerId === plexServerId);
}

function isLibrarySyncing(plexLibraryId: number): boolean {
	return get(libraryProgress).some((x) => x.id === plexLibraryId && !x.isComplete);
}

function openMediaPage(library: PlexLibraryDTO): void {
	switch (library.type) {
		case PlexMediaType.Movie:
			router.push(`/movies/${library.id}`);
			break;
		case PlexMediaType.TvShow:
			router.push(`/tvshows/${library.id}`);
			break;
		case PlexMediaType.Music:
			router.push(`/music/${library.id}`);
			break;
		case PlexMediaType.Photos:
			router.push(`/photos/${library.id}`);
			break;
		default:
			Log.error(library.type + ' was neither a movie, tvshow or music library');
			router.push(`/unknown/${library.id}`);
	}
}

onMounted(() => {
	useSubscription(
		signalRStore
			.getAllLibraryProgress()
			.subscribe((data) => {
				set(libraryProgress, data);
			}),
	);
});
</script>

<style lang="scss">
.server-name {
  width: 190px;
  display: flex;
  line-height: 24px;
  align-content: center;
  text-overflow: ellipsis;
}

.server-panels {
  z-index: 0;

  &.theme--dark {
    .v-expansion-panel {
      background: rgba(0, 0, 0, 0.3);
    }
  }

  &.theme--light {
    .v-expansion-panel {
      background: rgba(255, 255, 255, 0.3);
    }
  }
}

.ps {
  height: 100%;
  width: 100%;
}
</style>
