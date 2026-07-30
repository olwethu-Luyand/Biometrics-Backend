import type { InputHTMLAttributes } from "react";

interface CheckboxProps extends Omit<InputHTMLAttributes<HTMLInputElement>, "type"> {
  label: string;
}

export default function Checkbox({ label, id, className = "", ...props }: CheckboxProps) {
  const inputId = id || label.toLowerCase().replace(/\s+/g, "-");

  return (
    <label
      htmlFor={inputId}
      className={`flex cursor-pointer items-center gap-2 text-sm text-primeoak-gray-text ${className}`}
    >
      <input
        id={inputId}
        type="checkbox"
        className="h-4 w-4 rounded border-primeoak-gray-border text-primeoak-blue focus:ring-primeoak-blue"
        {...props}
      />
      {label}
    </label>
  );
}
