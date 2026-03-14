import time
import signal
import pymem
import pymem.process
PROCESS_NAME = "Stronghold Crusader.exe"
BASE1_OFFSET = 0x00110FE8
OFFSETS1 = [0x10]
BASE2_OFFSET = 0x0016843C
OFFSETS2 = [0x20]
SLEEP_INTERVAL = 0.07
DATA_TYPE = 'int'
running = True
def signal_handler(sig, frame):
    global running
    print("\n[!] Stopping...")
    running = False
signal.signal(signal.SIGINT, signal_handler)
def resolve_final_address(pm, module_name, base_offset, offsets):
    module = pymem.process.module_from_name(pm.process_handle, module_name)
    base_addr = module.lpBaseOfDll + base_offset
    addr = pm.read_int(base_addr)
    for offset in offsets[:-1]:
        addr = pm.read_int(addr + offset)
    final_addr = addr + offsets[-1] if offsets else base_addr
    return final_addr
def read_value(pm, addr):
    if DATA_TYPE == 'int':
        return pm.read_int(addr)
    else:
        raise ValueError("Unsupported DATA_TYPE")
def write_value(pm, addr, val):
    if DATA_TYPE == 'int':
        pm.write_int(addr, int(val))
    else:
        raise ValueError("Unsupported DATA_TYPE")
def main():
    global running
    pm = None
    batch_remaining = 0
    prev_val_second = 0
    second_is_zero = True
    last_non_zero_first = None
    print("Running... (Press Ctrl+C to stop)")
    while running:
        try:
            if pm is None:
                pm = pymem.Pymem(PROCESS_NAME)
                batch_remaining = 0
                prev_val_second = 0
                second_is_zero = True
                last_non_zero_first = None
            try:
                addr_first = resolve_final_address(pm, PROCESS_NAME, BASE1_OFFSET, OFFSETS1)
                addr_second = resolve_final_address(pm, PROCESS_NAME, BASE2_OFFSET, OFFSETS2)
            except Exception:
                time.sleep(0.5)
                continue
            while running:
                try:
                    addr_first = resolve_final_address(pm, PROCESS_NAME, BASE1_OFFSET, OFFSETS1)
                    addr_second = resolve_final_address(pm, PROCESS_NAME, BASE2_OFFSET, OFFSETS2)
                    val_first = read_value(pm, addr_first)
                    val_second = read_value(pm, addr_second)
                except Exception:
                    try: pm.close_process()
                    except: pass
                    pm = None
                    break
                if val_second == 0:
                    second_is_zero = True
                    batch_remaining = 0
                    last_non_zero_first = None
                else:
                    if second_is_zero:
                        batch_remaining = val_second
                        second_is_zero = False
                    else:
                        diff = val_second - prev_val_second
                        if diff > 0:
                            batch_remaining += diff
                prev_val_second = val_second
                if not second_is_zero and val_first != 0:
                    last_non_zero_first = val_first
                if val_first == 0 and (not second_is_zero) and batch_remaining > 0 and last_non_zero_first is not None:
                    try:
                        write_value(pm, addr_first, last_non_zero_first)
                        batch_remaining -= 1
                    except:
                        try: pm.close_process()
                        except: pass
                        pm = None
                        break
                time.sleep(SLEEP_INTERVAL)
        except pymem.exception.ProcessNotFound:
            time.sleep(1)
            if pm:
                try: pm.close_process()
                except: pass
            pm = None
            continue
        except:
            time.sleep(1)
            if pm:
                try: pm.close_process()
                except: pass
            pm = None
            continue
    if pm:
        try: pm.close_process()
        except: pass
    print("Stopped.")
if __name__ == "__main__":
    main()