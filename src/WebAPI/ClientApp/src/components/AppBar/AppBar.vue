<template>
	<q-header class="app-bar">
		<QRow no-wrap>
			<q-toolbar class="app-bar">
				<q-toolbar-title>
					<q-btn
						flat
						round
						dense
						icon="mdi-menu"
						class="q-mr-sm"
						@click.stop="showNavigationDrawer" />
					<q-btn
						to="/"
						flat>
						<Logo
							:size="24"
							class="q-mr-md" />
						{{ t('general.name-version', { version: globalStore.getAppVersion }) }}
					</q-btn>
				</q-toolbar-title>

				<q-space />

				<AppBarProgressBar />

				<q-space />

				<q-btn
					icon="mdi-github"
					flat
					href="https://github.com/PlexRipper/PlexRipper"
					target="_blank" />

				<!-- Background Activity Toggle -->
				<BackgroundActivityToggleButton />

				<!-- Account Selector -->
				<AccountSelector />

				<!-- Notifications Selector -->
				<NotificationButton @toggle="showNotificationsDrawer" />
			</q-toolbar>
		</QRow>
	</q-header>
</template>

<script setup lang="ts">
import { useGlobalStore } from '@store';

const { t } = useI18n();
const globalStore = useGlobalStore();

const emit = defineEmits<{
	(e: 'show-navigation' | 'show-notifications'): void;
}>();

function showNavigationDrawer(): void {
	emit('show-navigation');
}

function showNotificationsDrawer(): void {
	emit('show-notifications');
}
</script>

<style lang="scss">
@import '@/assets/scss/variables.scss';

.app-bar {
  // @extend .glossy;
  height: $app-bar-height;
}

body {
  &.body--dark {
    .app-bar {
      background: rgba(255, 0, 0, 0.2) !important;
    }
  }

  &.body--light {
    .app-bar {
      background: rgba(255, 0, 0, 1) !important;
    }
  }
}
</style>
