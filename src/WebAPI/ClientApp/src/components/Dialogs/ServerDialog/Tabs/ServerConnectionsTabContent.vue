<template>
	<QText size="h5">
		{{ $t('components.server-dialog.tabs.server-connections.section-header') }}
	</QText>
	<q-list>
		<!-- Plex Connections -->
		<ServerConnectionDisplayRow
			v-for="connection in serverConnectionStore.getServerConnectionsByServerId(plexServerId).filter(x => !x.isCustom)"
			:key="connection.id"
			:connection="connection" />
		<!-- Separator -->
		<QSeparator />
		<!-- Custom Connections -->
		<ServerConnectionDisplayRow
			v-for="connection in serverConnectionStore.getServerConnectionsByServerId(plexServerId).filter(x => x.isCustom)"
			:key="connection.id"
			:connection="connection" />
		<!-- Add Connection -->
		<q-item>
			<!-- Connection Url -->
			<q-item-section tag="label">
				<BaseButton
					:label="$t('components.server-dialog.tabs.server-connections.add-connection-button')"
					@click="dialogStore.openAddConnectionDialog({ plexServerId, plexServerConnectionId: 0 })" />
			</q-item-section>
			<q-space />
		</q-item>
		<AddConnectionDialog />
	</q-list>
</template>

<script setup lang="ts">
import ServerConnectionDisplayRow from '@components/Dialogs/ServerDialog/Tabs/ServerConnectionDisplayRow.vue';
import { useServerConnectionStore, useDialogStore } from '#imports';

const dialogStore = useDialogStore();
const serverConnectionStore = useServerConnectionStore();

defineProps<{
	plexServerId: number;
	isVisible: boolean;
}>();
</script>
