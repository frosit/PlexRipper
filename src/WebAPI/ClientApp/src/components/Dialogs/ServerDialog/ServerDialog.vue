<template>
	<QCardDialog
		:name="DialogType.ServerSettingsDialog"
		content-height="80"
		:loading="loading"
		cy="server-dialog-cy"
		@opened="open"
		@closed="close">
		<template #title>
			<EditableText
				size="h5"
				bold="medium"
				:display-text="
					$t('components.server-dialog.header', {
						serverName: serverStore.getServerName(plexServer?.id ?? 0) ?? $t('general.error.unknown'),
					})
				"
				:value="serverStore.getServerName(plexServer?.id ?? 0)"
				@save="onServerAliasSave" />
		</template>
		<template #default>
			<QRow
				align="start"
				full-height>
				<QCol
					cols="auto"
					align-self="stretch">
					<!-- Tab Index -->
					<q-tabs
						v-model="tabIndex"
						vertical
						active-color="red">
						<!--	Server Data	Tab Header -->
						<q-tab
							name="server-data"
							icon="mdi-server"
							data-cy="server-dialog-tab-1"
							:label="$t('components.server-dialog.tabs.server-data.header')" />
						<!--	Server Connections Tab Header	-->
						<q-tab
							name="server-connection"
							icon="mdi-connection"
							data-cy="server-dialog-tab-2"
							:label="$t('components.server-dialog.tabs.server-connections.header')" />
						<!--	Server Configuration Tab Header	-->
						<q-tab
							name="server-config"
							icon="mdi-cog-box"
							data-cy="server-dialog-tab-3"
							:label="$t('components.server-dialog.tabs.server-config.header')" />
						<!--	Library Destinations Tab Header	-->
						<q-tab
							name="download-destinations"
							icon="mdi-folder-edit-outline"
							data-cy="server-dialog-tab-4"
							:label="$t('components.server-dialog.tabs.download-destinations.header')" />
						<!--	Server Commands Tab Header	-->
						<q-tab
							name="server-commands"
							icon="mdi-console"
							data-cy="server-dialog-tab-5"
							:label="$t('components.server-dialog.tabs.server-commands.header')" />
					</q-tabs>
				</QCol>
				<QCol
					align-self="stretch"
					class="tab-content inherit-all-height scroll">
					<!-- Tab Content -->
					<q-tab-panels
						v-model="tabIndex"
						animated
						vertical
						transition-prev="slide-down"
						transition-next="slide-up">
						<!-- Server Data Tab Content -->
						<q-tab-panel
							name="server-data"
							data-cy="server-dialog-tab-content-1">
							<ServerDataTabContent
								:plex-server="plexServer"
								:is-visible="isVisible" />
						</q-tab-panel>

						<!-- Server Connections Tab Content	-->
						<q-tab-panel
							name="server-connection"
							data-cy="server-dialog-tab-content-2">
							<ServerConnectionsTabContent
								:plex-server-id="plexServerId"
								:is-visible="isVisible" />
						</q-tab-panel>

						<!--	Server Configuration Tab Content	-->
						<q-tab-panel
							name="server-config"
							data-cy="server-dialog-tab-content-3">
							<ServerConfigTabContent :plex-server="plexServer" />
						</q-tab-panel>

						<!--	Library Download Destinations	Tab Content -->
						<q-tab-panel
							name="download-destinations"
							class="inherit-all-height"
							data-cy="server-dialog-tab-content-4">
							<ServerLibraryDestinationsTabContent
								:plex-server="plexServer"
								:plex-libraries="libraryStore.getLibrariesByServerId(plexServerId)" />
						</q-tab-panel>

						<!--	Server Commands -->
						<q-tab-panel
							name="server-commands"
							data-cy="server-dialog-tab-content-5">
							<ServerCommandsTabContent
								:plex-server="plexServer"
								:is-visible="isVisible" />
						</q-tab-panel>
					</q-tab-panels>
				</QCol>
			</QRow>
		</template>
		<template #actions>
			<QRow justify="between">
				<QCol cols="auto">
					<HideButton @click="dialogStore.openDialog(DialogType.ServerHideConfirmationDialog)" />
				</QCol>
				<QCol cols="auto">
					<BaseButton
						cy="server-dialog-close-btn"
						flat
						:label="$t('general.commands.close')"
						color="default"
						@click="close" />
				</QCol>
			</QRow>
			<!-- Hide Server Confirm Dialog -->
			<ConfirmationDialog
				:confirm-loading="confirmHideDialog"
				:name="DialogType.ServerHideConfirmationDialog"
				:title="$t('confirmation.hide-server.title')"
				:text="$t('confirmation.hide-server.text')"
				class="q-mr-md"
				@confirm="onServerHiddenSave" />
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import { get, set } from '@vueuse/core';
import { tap } from 'rxjs/operators';
import type { PlexServerDTO } from '@dto';
import { DialogType } from '@enums';
import { useServerStore, useLibraryStore, useDialogStore } from '#imports';

const serverStore = useServerStore();
const libraryStore = useLibraryStore();
const dialogStore = useDialogStore();

const confirmHideDialog = ref(false);

const loading = ref(false);
const tabIndex = ref<string>('server-data');
const plexServer = ref<PlexServerDTO | null>(null);
const plexServerId = ref<number>(0);

const isVisible = computed((): boolean => plexServerId.value > 0);

function open(event: unknown): void {
	const newPlexServerId = event as number;
	set(plexServerId, newPlexServerId);
	set(loading, true);

	set(plexServer, serverStore.getServer(newPlexServerId));
	set(loading, false);
}

function close(): void {
	dialogStore.closeDialog(DialogType.ServerSettingsDialog);
	set(plexServerId, 0);
	set(tabIndex, 'server-data');
}

function onServerAliasSave(serverAlias: string): void {
	useSubscription(serverStore.setServerAlias(get(plexServerId), serverAlias).subscribe());
}

function onServerHiddenSave(): void {
	useSubscription(
		serverStore
			.setServerHidden(get(plexServerId), true)
			.pipe(tap(() => close()))
			.subscribe(),
	);
}
</script>

<style lang="scss">
@import '@/assets/scss/variables.scss';
@import 'quasar/src/css/core/typography.sass';

.tab-content {
  max-height: calc(80vh - $q-card-dialog-title-height - $q-card-dialog-actions-height) !important;
}

.editable-text {
  &-item {
    padding-top: 0;
    padding-bottom: 0;
  }
}
</style>
