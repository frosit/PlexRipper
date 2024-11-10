<template>
	<QPage>
		<template v-if="downloadStore.getServersWithDownloads.length > 0">
			<!-- Download Toolbar -->
			<DownloadBar />

			<!--	The Download Table	-->
			<QRow
				justify="center">
				<QCol cols="12">
					<q-list>
						<DownloadsTable
							v-for="{ plexServer, downloads } in downloadStore.getServersWithDownloads"
							:key="plexServer.id"
							:download-rows="downloads"
							:plex-server="plexServer"
							@action="commandSwitch($event)" />
					</q-list>
				</QCol>
			</QRow>
			<DownloadDetailsDialog />
		</template>
		<QRow
			v-else
			justify="center">
			<QAlert>
				{{ $t('pages.downloads.no-downloads') }}
			</QAlert>
		</QRow>
	</QPage>
</template>

<script setup lang="ts">
import type { DownloadProgressDTO } from '@dto';
import { useDownloadStore, useDialogStore } from '#imports';

const downloadStore = useDownloadStore();
const dialogStore = useDialogStore();

// region single commands

function commandSwitch({ action, item }: { action: string; item: DownloadProgressDTO }) {
	const ids: string[] = [item.id];

	if (action === 'details') {
		dialogStore.openDownloadTaskDetailsDialog(item.id);
		return;
	}

	downloadStore.executeDownloadCommand(action, ids);
}

// endregion
</script>
