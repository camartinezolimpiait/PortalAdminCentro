<!--
  StepIndicator.svelte
  Replaces: CompraPin step progress indicators from Blazor
  DaisyUI steps component for multi-step processes
-->
<script lang="ts">
	let {
		steps = [],
		currentStep = 1
	}: {
		steps?: { paso: number; nombre: string; completado: boolean }[];
		currentStep?: number;
	} = $props();
</script>

<ul class="steps steps-horizontal w-full mb-6 overflow-x-auto">
	{#each steps as step}
		<li
			class="step {step.completado || step.paso <= currentStep ? 'step-primary' : ''}"
			class:font-bold={step.paso === currentStep}
		>
			<span class="text-xs sm:text-sm">{step.nombre}</span>
		</li>
	{/each}
</ul>
